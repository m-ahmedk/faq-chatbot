using Microsoft.EntityFrameworkCore;
using FAQDemo.API.Data;
using FAQDemo.API.Extensions;
using FAQDemo.API.Logging;
using Serilog;
using OpenAI;
using Pgvector; // required for vector support

var builder = WebApplication.CreateBuilder(args);

// Config: appsettings -> user secrets (dev) -> env vars
builder.Configuration
       .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
       .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
       .AddEnvironmentVariables();

// Local dev only -> still support user-secrets
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Logging
LogConfigurator.ConfigureLogging(builder.Configuration, builder.Environment.EnvironmentName);
builder.Host.UseSerilog();

// Services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

if (builder.Environment.IsEnvironment("Test")) { }
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString, o => o.UseVector()));
}

builder.Services.AddCustomBinders();
builder.Services.AddCustomJsonConverters();
builder.Services.AddSwaggerDocument();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddProjectRepositories();
builder.Services.AddProjectServices();
builder.Services.AddProjectValidators();
builder.Services.AddLogging(); // AutoMapper/Serilog compatibility
builder.Services.AddProjectMappings();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

// Ensure migrations and schema are applied
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // use Migrate() with Postgres instead of EnsureCreated()
}
await DbInitializer.Seed(app.Services);

app.Run();

// public partial class Program { }