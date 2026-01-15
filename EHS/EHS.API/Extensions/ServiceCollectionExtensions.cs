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
using EHS.Infrastructure.Services.Email;
using FluentEmail.MailKitSmtp;
using EHS.Infrastructure.Services.Storage;

namespace EHS.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Default"));
            });
            return services;
        }

        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
            .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JWTOptions>(configuration.GetSection("JWT"));

            services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidAudience = configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JWT:Key"] ??
                        throw new InvalidOperationException("JWT:Key not configured")))
                };
            });

            return services;
        }

        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            // Master Data Validators
            services.AddScoped<IValidator<CreateOrganizationRequest>, CreateOrganizationRequestValidator>();
            services.AddScoped<IValidator<UpdateOrganizationRequest>, UpdateOrganizationRequestValidator>();
            services.AddScoped<IValidator<CreateIncidentTypeRequest>, CreateIncidentTypeRequestValidator>();
            services.AddScoped<IValidator<UpdateIncidentTypeRequest>, UpdateIncidentTypeRequestValidator>();
            services.AddScoped<IValidator<CreateIncidentNatureRequest>, CreateIncidentNatureRequestValidator>();
            services.AddScoped<IValidator<UpdateIncidentNatureRequest>, UpdateIncidentNatureRequestValidator>();
            services.AddScoped<IValidator<CreateIncidentSeverityRequest>, CreateIncidentSeverityRequestValidator>();
            services.AddScoped<IValidator<UpdateIncidentSeverityRequest>, UpdateIncidentSeverityRequestValidator>();
            services.AddScoped<IValidator<CreateDepartmentRequest>, CreateDepartmentRequestValidator>();
            services.AddScoped<IValidator<UpdateDepartmentRequest>, UpdateDepartmentRequestValidator>();
            services.AddScoped<IValidator<CreateProductionLineRequest>, CreateProductionLineRequestValidator>();
            services.AddScoped<IValidator<UpdateProductionLineRequest>, UpdateProductionLineRequestValidator>();
            services.AddScoped<IValidator<CreateMachineRequest>, CreateMachineRequestValidator>();
            services.AddScoped<IValidator<UpdateMachineRequest>, UpdateMachineRequestValidator>();

            // Incident Workflow Validators
            services.AddScoped<IValidator<CreateIncidentRequest>, CreateIncidentRequestValidator>();
            services.AddScoped<IValidator<UpdateIncidentRequest>, UpdateIncidentRequestValidator>();
            services.AddScoped<IValidator<RejectIncidentRequest>, RejectIncidentRequestValidator>();
            services.AddScoped<IValidator<ReassignToInitiatorRequest>, ReassignToInitiatorRequestValidator>();
            services.AddScoped<IValidator<AcceptAndAssignRequest>, AcceptAndAssignRequestValidator>();
            services.AddScoped<IValidator<CloseIncidentRequest>, CloseIncidentRequestValidator>();
            services.AddScoped<IValidator<AcceptIncidentRequest>, AcceptIncidentRequestValidator>();
            services.AddScoped<IValidator<PassToPeerRequest>, PassToPeerRequestValidator>();
            services.AddScoped<IValidator<UpdateImplementationRequest>, UpdateImplementationRequestValidator>();

            return services;
        }

        public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Organization>, Repository<Organization>>();
            services.AddScoped<IRepository<Department>, Repository<Department>>();
            services.AddScoped<IRepository<ProductionLine>, Repository<ProductionLine>>();
            services.AddScoped<IRepository<Machine>, Repository<Machine>>();
            services.AddScoped<IRepository<IncidentType>, Repository<IncidentType>>();
            services.AddScoped<IRepository<IncidentNature>, Repository<IncidentNature>>();
            services.AddScoped<IRepository<IncidentSeverity>, Repository<IncidentSeverity>>();

            // Incident Workflow Repositories
            services.AddScoped<IRepository<Incident>, Repository<Incident>>();
            services.AddScoped<IRepository<IncidentStatus>, Repository<IncidentStatus>>();
            services.AddScoped<IRepository<IncidentImplementation>, Repository<IncidentImplementation>>();
            services.AddScoped<IRepository<RootCauseAnalysisDetail>, Repository<RootCauseAnalysisDetail>>();
            services.AddScoped<IRepository<ImplementationBenefit>, Repository<ImplementationBenefit>>();
            services.AddScoped<IRepository<IncidentComment>, Repository<IncidentComment>>();
            services.AddScoped<IRepository<IncidentAttachment>, Repository<IncidentAttachment>>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();

            // Master Data Services
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IIncidentTypeService, IncidentTypeService>();
            services.AddScoped<IIncidentNatureService, IncidentNatureService>();
            services.AddScoped<IIncidentSeverityService, IncidentSeverityService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IProductionLineService, ProductionLineService>();
            services.AddScoped<IMachineService, MachineService>();

            // Incident Workflow Services
            services.AddScoped<IIncidentService, IncidentService>();

            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
                .AddRazorRenderer()
                .AddMailKitSender(new SmtpClientOptions
                {
                    Server = emailSettings.SmtpHost,
                    Port = emailSettings.SmtpPort,
                    User = emailSettings.SmtpUsername,
                    Password = emailSettings.SmtpPassword,
                    UseSsl = true,
                    RequiresAuthentication = true,
                    SocketOptions = MailKit.Security.SecureSocketOptions.StartTls
                });

            // For MailKit (more robust):
            // services.AddFluentEmail(...)
            //    .AddRazorRenderer()
            //    .AddMailKitSender(new FluentEmail.MailKit.Smtp.SmtpClientOptions { ... });
            // Since I installed FluentEmail.MailKit, I should use it.
            // However, typical FluentEmail setup uses SmtpClient for simplicity or specific MailKit extensions.
            // Let's stick to the basic SmtpSender for now if MailKit extension is not showing up or needs more config.
            // Actually, I installed `FluentEmail.MailKit`. Let me check if `AddMailKitSender` is available.
            // It should be. But I'll use standard SmtpClient for the first pass to be safe if I don't recall the exact MailKit options class.
            // Wait, the user specifically asked for MailKit.
            // I should try to use it.

            services.AddSingleton<EmailChannel>();
            services.AddScoped<ISendEmailService, EmailService>();
            services.AddHostedService<EmailBackgroundService>();

            return services;
        }

        public static IServiceCollection AddFileStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
            services.AddHttpContextAccessor(); // Needed for Local Storage URL generation

            var settings = configuration.GetSection("FileStorage").Get<FileStorageSettings>();

            if (settings != null && settings.Provider.Equals("Azure", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            }
            else
            {
                services.AddScoped<IFileStorageService, LocalFileStorageService>();
            }

            return services;
        }

        public static IServiceCollection AddAutoMapperProfiles(this IServiceCollection services)
        {
            services.AddAutoMapper(c => { }, typeof(MasterDataMappingProfile).Assembly);
            services.AddAutoMapper(c => { }, typeof(IncidentMappingProfile).Assembly);

            return services;
        }
    }
}