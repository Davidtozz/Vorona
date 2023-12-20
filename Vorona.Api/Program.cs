using System.Diagnostics;
using Vorona.Api.Hubs;
using Vorona.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Vorona.Api.Extensions;
using Microsoft.AspNetCore.Mvc;
using Vorona.Api.Data;
using Microsoft.EntityFrameworkCore;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;
using Vorona.Api.Entities;
using User = Vorona.Api.Entities.User;


const string DEBUG_PREFIX = "\x1b[31mdbug:\x1b[0m";

//! BREAKING CHANGE: Slim builder, less boilerplate
var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<PostgresContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDB"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<UserTracker>();
//builder.Services.AddScoped<PostgresContext>();
builder.Services.AddHttpLogging(o => { });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin();
    });
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.WriteIndented = true;
});

//? Extension 
builder.Services.ConfigureJwtAuthentication(configuration: builder.Configuration);

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});

var app = builder.Build();
app.UseHttpLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.MapPost("/register", (
    [FromBody] User user,
    PostgresContext db,
    HttpContext ctx
) =>
{

    var passwordHash = SecretHasher.Hash(user.Password);
    var refreshToken = Guid.NewGuid().ToString();
    
    user.RefreshToken = refreshToken;
    user.Password = passwordHash;

    ctx.Response.Cookies.Append("_rtkId", refreshToken, new CookieOptions
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        Secure = false,
        Path = "/"
    });

    
    var transaction = db.Database.BeginTransaction();

    try {
        db.Users.Add(user);
        transaction.Commit();
        db.SaveChanges();
        return Results.Ok(user);
    } catch(DbUpdateException) {
        transaction.Rollback();
        return Results.BadRequest();
    }
});

app.MapPost("/api/v1/jwt", async (
    [FromBody] User user, 
    HttpContext ctx) =>
{
    if (string.IsNullOrWhiteSpace(user.Username))
    {
        ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
        Console.WriteLine($"{DEBUG_PREFIX} Username is required");
        await ctx.Response.WriteAsync("Username is required");
        return;
    }
    
    string jwtToken = GenerateJwtToken(user.Username);
    ctx.Response.Cookies.Append("X-Access-Token", jwtToken, new CookieOptions
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        Secure = false,
        Path = "/"
    });

    ctx.Response.ContentType = "text/json";
    await ctx.Response.WriteAsJsonAsync(new { Token = jwtToken });
}).AllowAnonymous();
app.MapHub<MessageHub>("/chat");
app.Run();




string GenerateJwtToken(string username, string role = "user")
{
    var tokenHandler = new JwtSecurityTokenHandler();
    var securityKey = Encoding.ASCII.GetBytes(app.Configuration["Jwt:Key"]!);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new("unique_name", username), //TODO: Replace with actual username
            new("role", role)
        }),
        Expires = DateTime.UtcNow.AddMinutes(5),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey), SecurityAlgorithms.HmacSha256Signature),
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}