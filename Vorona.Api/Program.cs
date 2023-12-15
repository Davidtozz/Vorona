using Microsoft.Extensions.Options;
using Vorona.Api.Hubs;
using Vorona.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Authorization;
using Vorona.Api.Extensions;

//! BREAKING CHANGE: Slim builder, less boilerplate
var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddSingleton<UserTracker>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyMethod().AllowAnyHeader().AllowCredentials().WithOrigins("http://localhost:5173");
    });
});


//? Extension 
builder.Services.ConfigureJwtAuthentication(configuration: builder.Configuration);

builder.WebHost.UseKestrel(options =>
{
    options.AddServerHeader = false;
});



var app = builder.Build();




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

app.MapGet("/", () => "Hello World!");
app.MapGet("/api/v1/health", () => new { Status = "Healthy", Version = "v1", Environment = app.Environment.EnvironmentName });
app.MapGet("/api/v1/users", (UserTracker userTracker) => userTracker.Users);

app.MapGet("/api/v1/protected", () => "Hello World!")
.RequireAuthorization(
    new AuthorizeAttribute
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "Admin"
    }
).Produces<string>(contentType: "text/plain");

//? Bug: route not mapped in SwaggerUI
app.MapGet("/api/v1/jwt", async (ctx) =>
{
    #region TokenGeneration
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(app.Configuration["Jwt:Key"]!);
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new Claim[]
        {
            new(ClaimTypes.Name, "TestUser"), //TODO: Replace with actual username
            new(ClaimTypes.Role, "Admin")
        }),
        Expires = DateTime.UtcNow.AddMinutes(5),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
    
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    #endregion
    
    string tokenString = tokenHandler.WriteToken(token);
    ctx.Response.Cookies.Append("X-Access-Token", tokenString, new CookieOptions
    {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict,
        Secure = false,
        Path = "/"
    });
    ctx.Response.ContentType = "text/json";
    await ctx.Response.WriteAsJsonAsync(new { Token = tokenString });
}).AllowAnonymous();
app.MapHub<MessageHub>("/chat");
app.Run();
