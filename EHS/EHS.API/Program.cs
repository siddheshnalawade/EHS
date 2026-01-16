using EHS.API.Extensions;
using EHS.API.Middlewares;
using Serilog;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Configure Serilog to read from appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

// Clear default logging providers and add Serilog
builder.Host.UseSerilog();

// ============================================
// Database Configuration
// ============================================
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// ============================================
// Identity & Password Policy Configuration
// ============================================
builder.Services.AddIdentityConfiguration();

// Todo: Implement Rate Limiter

// ============================================
// Authorization Policy Configuration
// ============================================
/// <summary>
/// Adds authorization services for role-based access control.
/// Policies can be defined here for granular permission management.
/// </summary>
builder.Services.AddAuthorization();

// ============================================
// JWT Configuration
// ============================================
// ============================================
// Azure AD Auth Configuration
// ============================================
builder.Services.AddAzureAdAuthentication(builder.Configuration);

// ============================================
// AutoMapper Profile Cofiguration
// ============================================
builder.Services.AddAutoMapperProfiles();

// ============================================
// Validator Configuration
// ============================================
builder.Services.AddApplicationValidators();

// ============================================
// Repository Registration
// ============================================
builder.Services.AddApplicationRepositories();

// ============================================
// Service Registration
// ============================================
// ============================================
// ============================================
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEmailServices(builder.Configuration);
builder.Services.AddFileStorageServices(builder.Configuration);

// Todo - Add CORS policy
// Todo - Add Swagger for API documentation

// ============================================
// Build Application
// ============================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "EHS");
    });
}

/// <summary>
/// Enable Serilog Request Logging.
/// Logs HTTP requests with clearer messages and performance data.
/// </summary>
app.UseSerilogRequestLogging();

/// <summary>
/// Global exception handling middleware.
/// Catches and handles exceptions in a centralized manner.
/// </summary>
app.UseMiddleware<ApplicationExceptionHandlingMiddleware>();

/// <summary>
/// Authentication middleware.
/// Validates JWT tokens and establishes user principal.
/// Must come before Authorization.
/// </summary>
app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>();

/// <summary>
/// Authorization middleware.
/// Checks if authenticated user has required permissions.
/// Must come after Authentication.
/// </summary>
app.UseAuthorization();

/// <summary>
/// Maps controller routes.
/// Registers all API endpoints defined in controllers.
/// </summary>
app.MapControllers();
app.MapHealthChecks("/health");

/// <summary>
/// Runs the application asynchronously.
/// Allows for graceful shutdown handling.
/// </summary>
await app.RunAsync();