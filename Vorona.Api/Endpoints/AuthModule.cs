using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using Vorona.Api.Data;
using Vorona.Api.Entities;
using Vorona.Api.Models;

namespace Vorona.Api.Endpoints;

public partial class AuthModule : CarterModule
{
    private readonly IConfiguration _configuration;

    public AuthModule(IConfiguration configuration) : base("/api/v1/auth")
    {
        _configuration = configuration;
        this.WithTags("/auth");
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/signup", Signup);
        app.MapPost("/login", Login);
        #warning The Jwt endpoint is deprecated and should not be used outside testing
        app.MapPost("/jwt", Jwt); 
    }

    [Obsolete("This endpoint is deprecated and should not be used")]
    [SwaggerOperation(
        Summary = "(TEST ONLY) Returns a JWT token",
        Description = "(TEST ONLY) Returns a JWT token. Should not be used in production")]
    private IResult Jwt([FromBody] LoginModel user, HttpContext ctx)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
        {
            return Results.BadRequest("Username is required");
        }

        string jwtToken = GenerateJwtToken(user.Username);
        ctx.Response.Cookies.Append("X-Access-Token", jwtToken, new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = false
        });
        
        return Results.Ok(new { Token = jwtToken });
    }

    #region SwaggerDocs
    [SwaggerOperation(Summary = "Logs in a user; returns a JWT token", Description = "Note: Working as expected!")]
    [SwaggerResponse(200, "The user was logged in successfully")]
    [SwaggerResponse(401, "The user is not registered")]
    [SwaggerResponse(500, "Internal server error")]
    [AllowAnonymous]
    #endregion
    private async Task<IResult> Login([FromBody] LoginModel user, PostgresContext db, HttpContext ctx)
    {
        var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
        if (existingUser is default(User))
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
                //? Retrieve the existing token 
                var existingToken = await db.Tokens.FirstOrDefaultAsync(t => t.UserId == existingUser.Id);

                if (existingToken is not default(Token))
                {
                    //? Update the existing token
                    await db.Tokens
                        .Where(t => t.UserId == existingUser.Id)
                        .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.Id, Guid.NewGuid()));
                } else throw new Exception("Token was not found in the database");
                    
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return Results.BadRequest($"Something went wrong: {e.Message}");
            }
                
            return Results.Ok(new { AccessToken = jwtToken });
        }
        else
        {
            await Task.Delay(1000);
            return Results.Unauthorized();
        }
    }
    
    #region SwaggerDocs
    [SwaggerOperation(Summary = "Registers a new user", Description = "Registers a new user. Returns a JWT token")]
    [SwaggerResponse(200, "The user was registered successfully")]
    [SwaggerResponse(409, "The user is already registered")]
    [SwaggerResponse(500, "Internal server error")]
    [AllowAnonymous]
    #endregion
    private async Task<IResult> Signup([FromBody] SignupModel user, PostgresContext db, HttpContext ctx)
    {
        //check if the user is already registered
        var existingUser = db.Users.FirstOrDefault(u => u.Username == user.Username);
        if (existingUser is not null)
        {
            return Results.Conflict("Username already exists");
        }

        User newUser = new()
        {
            Username = user.Username,
            Password = SecretHasher.Hash(user.Password),
            Email = user.Email,

        };
            
        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Strict,
            Secure = false
        };
            
        string jwtToken = GenerateJwtToken(newUser.Username);
            
        var transaction = db.Database.BeginTransaction();

        try
        {
            await db.Users.AddAsync(newUser);
            await transaction.CommitAsync();
            await db.SaveChangesAsync();
                
            Token token = new Token
            {
                UserId = newUser.Id,
                ExpiresAt = DateTime.Now.AddDays(7)
            };
            newUser.Token = token;

            await db.Tokens.AddAsync(token);
            await db.SaveChangesAsync();
            await transaction.DisposeAsync();
            ctx.Response.Cookies.Append("X-Access-Token", jwtToken, cookieOptions);
            ctx.Response.Cookies.Append("_rtkId", newUser.Token.Id.ToString(), cookieOptions);
            return Results.Ok(user);
        }
        catch (DbUpdateException)
        {
            transaction.Rollback();
            return Results.BadRequest();
        }
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
            Expires = DateTime.Now.AddMinutes(5),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}