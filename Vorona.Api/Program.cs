using Vorona.Api.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}


app.MapGet("/", () => "Hello World!");
app.MapGet("/api/v1/health", () => new { Status = "Healthy", Version = "v1", Environment = app.Environment.EnvironmentName });
app.MapHub<MessageHub>("/chat");
app.Run();
