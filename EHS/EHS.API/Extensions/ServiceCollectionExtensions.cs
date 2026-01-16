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

        public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Requires: Microsoft.Identity.Web
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApi(configuration.GetSection("AzureAd"));

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

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<AzureUserSyncService>(); // Sync User Middleware Service

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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILookupService, LookupService>();

            // SMS Services
            var smsSettings = configuration.GetSection("SmsSettings").Get<SmsSettings>();
            if (smsSettings != null && smsSettings.UseAzureServiceBus)
            {
                 services.AddScoped<ISmsQueueProducer, EHS.Infrastructure.Services.Sms.AzureServiceBusSmsProducer>();
                 services.AddHostedService<EHS.Infrastructure.Services.Sms.AzureServiceBusSmsConsumer>();
            }
            else
            {
                services.AddSingleton<EHS.Infrastructure.Services.Sms.SmsChannel>();
                services.AddScoped<ISmsQueueProducer, EHS.Infrastructure.Services.InMemorySmsQueueProducer>();
                services.AddHostedService<EHS.Infrastructure.Services.SmsBackgroundService>(); 
            }
            services.AddScoped<ISmsService, SmsService>();

            return services;
        }

        public static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
                .AddRazorRenderer();

            if (!string.IsNullOrEmpty(emailSettings.CommunicationServiceConnectionString))
            {
                // Use Azure Communication Services for Sending
                services.AddScoped<FluentEmail.Core.Interfaces.ISender, AzureCommunicationEmailSender>();
            }
            else
            {
                // Use SMTP (MailKit)
                services.AddFluentEmail(emailSettings.FromEmail, emailSettings.FromName)
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
            }

            if (emailSettings.UseAzureServiceBus)
            {
                // Azure Service Bus Implementation
                services.AddScoped<IEmailQueueProducer, AzureServiceBusEmailProducer>();
                services.AddHostedService<AzureServiceBusEmailConsumer>();
            }
            else
            {
                // In-Memory Channel Implementation
                services.AddSingleton<EmailChannel>();
                services.AddScoped<IEmailQueueProducer, InMemoryEmailQueueProducer>();
                services.AddHostedService<EmailBackgroundService>();
            }

            services.AddScoped<ISendEmailService, EmailService>();

            return services;
        }

        public static IServiceCollection AddFileStorageServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));
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