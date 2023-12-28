using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Vorona.Api.Data;
using Vorona.Api.Entities;

namespace Vorona.Api.Endpoints;

public class AuthModule : CarterModule
{
    private readonly IConfiguration _configuration;

    public AuthModule(IConfiguration configuration) : base("/api/v1/auth")
    {
        _configuration = configuration;
        this.WithTags("/auth");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/signup",
        #region SwaggerDocs
        [SwaggerOperation(
            Summary = "Registers a new user",
            Description = "Registers a new user. Returns a JWT token"
        )]
        [SwaggerResponse(200, "The user was registered successfully")]
        [SwaggerResponse(409, "The user is already registered")]
        [SwaggerResponse(500, "Internal server error")]
        #endregion
        [AllowAnonymous] (
            [FromBody] User user,
            PostgresContext db,
            HttpContext ctx
        ) =>
        {
            //check if the user is already registered
            var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser is not null)
            {
                return Results.Conflict("Username already exists");
            }

            user.Token = new Token
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ExpiresAt = DateTime.Now.AddDays(7)
            };
            user.Password = SecretHasher.Hash(user.Password);

            ctx.Response.Cookies.Append("_rtkId", user.Token.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                Path = "/"
            });
            
            var transaction = db.Database.BeginTransaction();

            try
            {
                db.Users.Add(user);
                transaction.Commit();
                db.SaveChanges();
                return Results.Ok(user);
            }
            catch (DbUpdateException)
            {
                transaction.Rollback();
                return Results.BadRequest();
            }
        });
        
        app.MapPost("/login", async (
            [FromBody] User user, 
            PostgresContext db,
            HttpContext ctx
            ) =>
        {
            var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
            if (existingUser is null)
            {
                return Results.NotFound();
            }
            
            if (SecretHasher.Verify(user.Password, existingUser.Password))
            {
                var jwtToken = GenerateJwtToken(existingUser.Username);
                ctx.Response.Cookies.Append("X-Access-Token", jwtToken, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = false
                });
                
                var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    user.Token = new Token()
                    {
                        Id = Guid.NewGuid(),
                        UserId = existingUser.Id,
                        ExpiresAt = DateTime.Now.AddDays(7)
                    };
                    
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                       await transaction.RollbackAsync();
                       return Results.BadRequest("Something went wrong");
                }
                
                return Results.Ok(new { AccessToken = jwtToken });
            }
            else
            {
                await Task.Delay(1000);
                return Results.Unauthorized();
            }
        });

        // ! deprecated
        app.MapPost("/jwt",
        [Obsolete("This endpoint is deprecated and should not be used in production")]
        [SwaggerOperation(
            Summary = "(TEST ONLY) Returns a JWT token",
            Description = "(TEST ONLY) Returns a JWT token. Should not be used in production"
        )]
        async ([FromBody] User user, HttpContext ctx) =>
        {
            if (string.IsNullOrWhiteSpace(user.Username))
            {
                ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
                await ctx.Response.WriteAsync("Username is required");
                return;
            }

            string jwtToken = GenerateJwtToken(user.Username);
            ctx.Response.Cookies.Append("X-Access-Token", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false
            });

            ctx.Response.ContentType = "text/json";
            await ctx.Response.WriteAsJsonAsync(new { Token = jwtToken });
        });
    }

    private string GenerateJwtToken(string username, string role = "user")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new("username", username),
                new("role", role)
            }),
            Expires = DateTime.UtcNow.AddSeconds(30),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}