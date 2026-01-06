using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using EHS.Domain.Settings;
using EHS.Infrastructure.Data;
using EHS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ============================================
// Database Configuration
// ============================================
/// <summary>
/// Adds Entity Framework Core with SQL Server.
/// Connection string sourced from appsettings.json
/// </summary>
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// ============================================
// JWT Configuration
// ============================================
/// <summary>
/// Maps JWT settings from appsettings.json to JWTOptions class.
/// Used to configure token expiration, issuer, audience, and signing key.
/// </summary>
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

// ============================================
// Authentication Service Registration
// ============================================
/// <summary>
/// Registers AuthService as transient.
/// Each request gets a new instance to avoid state sharing issues.
/// Implements IAuthService interface for dependency injection.
/// </summary>
builder.Services.AddTransient<IAuthService, AuthService>();

// ============================================
// Identity & Password Policy Configuration
// ============================================
/// <summary>
/// Configures ASP.NET Core Identity with strong security defaults.
/// Enforces secure password policies and account lockout mechanisms.
/// </summary
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequiredLength = 12;
    options.Password.RequiredUniqueChars = 4;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;

    // Todo : Account lockout settings

    // User settings
    options.User.RequireUniqueEmail = true;
    // Todo : Confirmed email settings
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
    .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

// Todo: Implement Rate Limiter

// ============================================
// JWT Authentication Configuration
// ============================================
/// <summary>
/// Configures JWT Bearer authentication for API endpoints.
/// Validates token signature, issuer, audience, and lifetime.
/// ClockSkew of 0 means no tolerance for expired tokens.
/// </summary>
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidAudience = builder.Configuration["JWT:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"] ??
                throw new InvalidOperationException("JWT:Key not configured")))
        };
    });

// ============================================
// Authorization Policy Configuration
// ============================================
/// <summary>
/// Adds authorization services for role-based access control.
/// Policies can be defined here for granular permission management.
/// </summary>
builder.Services.AddAuthorization();

// ============================================
// Build Application
// ============================================
var app = builder.Build();

/// <summary>
/// Authentication middleware.
/// Validates JWT tokens and establishes user principal.
/// Must come before Authorization.
/// </summary>
app.UseAuthentication();

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

/// <summary>
/// Runs the application asynchronously.
/// Allows for graceful shutdown handling.
/// </summary>
await app.RunAsync();