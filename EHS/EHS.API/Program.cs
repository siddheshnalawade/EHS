using EHS.API.Middlewares;
using EHS.Application.DTOs;
using EHS.Application.Interfaces;
using EHS.Application.Mappings;
using EHS.Application.Repositories;
using EHS.Application.Validators;
using EHS.Domain.Entities;
using EHS.Domain.Settings;
using EHS.Infrastructure.Data;
using EHS.Infrastructure.Repositories;
using EHS.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

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
// JWT Configuration
// ============================================
/// <summary>
/// Maps JWT settings from appsettings.json to JWTOptions class.
/// Used to configure token expiration, issuer, audience, and signing key.
/// </summary>
builder.Services.Configure<JWTOptions>(builder.Configuration.GetSection("JWT"));

// ============================================
// AutoMapper Profile Cofiguration
// ============================================
builder.Services.AddAutoMapper(c => { }, typeof(OrganizationMappingProfile).Assembly);

// ============================================
// Validator Configuration
// ============================================
builder.Services.AddScoped<IValidator<CreateOrganizationRequest>, CreateOrganizationRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateOrganizationRequest>, UpdateOrganizationRequestValidator>();

// Master Data Validators
builder.Services.AddScoped<IValidator<CreateIncidentTypeRequest>, CreateIncidentTypeRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateIncidentTypeRequest>, UpdateIncidentTypeRequestValidator>();
builder.Services.AddScoped<IValidator<CreateIncidentNatureRequest>, CreateIncidentNatureRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateIncidentNatureRequest>, UpdateIncidentNatureRequestValidator>();
builder.Services.AddScoped<IValidator<CreateIncidentSeverityRequest>, CreateIncidentSeverityRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateIncidentSeverityRequest>, UpdateIncidentSeverityRequestValidator>();
builder.Services.AddScoped<IValidator<CreateDepartmentRequest>, CreateDepartmentRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateDepartmentRequest>, UpdateDepartmentRequestValidator>();
builder.Services.AddScoped<IValidator<CreateProductionLineRequest>, CreateProductionLineRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateProductionLineRequest>, UpdateProductionLineRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMachineRequest>, CreateMachineRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMachineRequest>, UpdateMachineRequestValidator>();

// Incident Workflow Validators
builder.Services.AddScoped<IValidator<CreateIncidentRequest>, CreateIncidentRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateIncidentRequest>, UpdateIncidentRequestValidator>();
builder.Services.AddScoped<IValidator<RejectIncidentRequest>, RejectIncidentRequestValidator>();
builder.Services.AddScoped<IValidator<ReassignToInitiatorRequest>, ReassignToInitiatorRequestValidator>();
builder.Services.AddScoped<IValidator<AcceptAndAssignRequest>, AcceptAndAssignRequestValidator>();
builder.Services.AddScoped<IValidator<CloseIncidentRequest>, CloseIncidentRequestValidator>();
builder.Services.AddScoped<IValidator<AcceptIncidentRequest>, AcceptIncidentRequestValidator>();
builder.Services.AddScoped<IValidator<PassToPeerRequest>, PassToPeerRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateImplementationRequest>, UpdateImplementationRequestValidator>();

// ============================================
// Repository Registration
// ============================================
builder.Services.AddScoped<IRepository<Organization>, Repository<Organization>>();
builder.Services.AddScoped<IRepository<Department>, Repository<Department>>();
builder.Services.AddScoped<IRepository<ProductionLine>, Repository<ProductionLine>>();
builder.Services.AddScoped<IRepository<Machine>, Repository<Machine>>();
builder.Services.AddScoped<IRepository<IncidentType>, Repository<IncidentType>>();
builder.Services.AddScoped<IRepository<IncidentNature>, Repository<IncidentNature>>();
builder.Services.AddScoped<IRepository<IncidentSeverity>, Repository<IncidentSeverity>>();

// Incident Workflow Repositories
builder.Services.AddScoped<IRepository<Incident>, Repository<Incident>>();
builder.Services.AddScoped<IRepository<IncidentStatus>, Repository<IncidentStatus>>();
builder.Services.AddScoped<IRepository<IncidentImplementation>, Repository<IncidentImplementation>>();
builder.Services.AddScoped<IRepository<RootCauseAnalysisDetail>, Repository<RootCauseAnalysisDetail>>();
builder.Services.AddScoped<IRepository<ImplementationBenefit>, Repository<ImplementationBenefit>>();

// ============================================
// Service Registration
// ============================================
/// <summary>
/// Registers all application services as Scoped.
/// Scoped lifetime ensures one instance per HTTP request,
/// which aligns with DbContext lifetime.
/// </summary>
builder.Services.AddScoped<IAuthService, AuthService>();

// Master Data Services
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IIncidentTypeService, IncidentTypeService>();
builder.Services.AddScoped<IIncidentNatureService, IncidentNatureService>();
builder.Services.AddScoped<IIncidentSeverityService, IncidentSeverityService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IProductionLineService, ProductionLineService>();
builder.Services.AddScoped<IMachineService, MachineService>();

// Incident Workflow Services
builder.Services.AddScoped<IIncidentService, IncidentService>();

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
/// Global exception handling middleware.
/// </summary>
app.UseMiddleware<ApplicationExceptionHandlingMiddleware>();

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