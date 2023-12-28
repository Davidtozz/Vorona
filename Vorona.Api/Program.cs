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
using Npgsql;
using Microsoft.AspNetCore.SignalR;
using Carter;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;


/* const string DEBUG_PREFIX = "\x1b[31mdbug:\x1b[0m"; */

//! BREAKING CHANGE: Slim builder, less boilerplate
var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddDbContext<PostgresContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDB"));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("user", policyBuilder =>
    {
        policyBuilder.RequireRole("user");
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
    });
    
    options.AddPolicy("admin", policyBuilder =>
    {
        policyBuilder.RequireRole("admin");
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
    options.SwaggerDoc("v1", new() { Title = "Vorona API", Version = "v1" });
    options.EnableAnnotations();
}
);
builder.Services.AddSignalR();
builder.Services.AddCarter();

builder.Services.AddSingleton<UserTracker>();
//builder.Services.AddScoped<PostgresContext>();
builder.Services.AddHttpLogging(o => { });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
    {
        p.WithOrigins("http://localhost:5173")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
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
app.UseCors(policyName: "AllowAll");
app.UseHttpLogging();


//app.UseMiddleware<AuthMiddleware>();

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

//? Map carter modules (endpoints)
app.MapCarter();

app.MapHub<MessageHub>("/chat");
app.Run();