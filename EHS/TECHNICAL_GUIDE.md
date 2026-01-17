# EHS System - Technical Implementation Guide

## 🏗️ Project Structure

```
EHS/
├── EHS.API/                          # Presentation Layer
│   ├── Controllers/v1/               # API Controllers
│   │   ├── AuthController.cs         # Authentication endpoints
│   │   ├── IncidentsController.cs    # Initiator operations
│   │   ├── SafetyOfficerController.cs # Safety officer operations
│   │   ├── ImplementorController.cs  # Implementor operations
│   │   ├── DepartmentsController.cs  # Master data
│   │   ├── MachinesController.cs     # Master data
│   │   ├── OrganizationsController.cs # Master data
│   │   ├── ProductionLinesController.cs # Master data
│   │   ├── IncidentTypesController.cs # Master data
│   │   ├── IncidentNaturesController.cs # Master data
│   │   ├── IncidentSeveritiesController.cs # Master data
│   │   ├── StorageController.cs      # File upload
│   │   └── IncidentAttachmentsController.cs # File management
│   ├── Middlewares/
│   │   ├── ApplicationExceptionHandlingMiddleware.cs # Global error handling
│   │   └── UserSyncMiddleware.cs     # Azure AD user sync
│   ├── Templates/                    # Email templates
│   │   ├── IncidentCreated.cshtml
│   │   ├── IncidentAssigned.cshtml
│   │   ├── IncidentRejected.cshtml
│   │   └── IncidentReassigned.cshtml
│   ├── Extensions/
│   │   └── ServiceCollectionExtensions.cs # DI configuration
│   ├── Program.cs                    # Application entry point
│   └── appsettings.json              # Configuration
│
├── EHS.Application/                  # Application Layer
│   ├── DTOs/                         # Data Transfer Objects
│   │   ├── ApiResponse.cs
│   │   ├── IncidentDto.cs
│   │   ├── ImplementorDto.cs
│   │   ├── SafetyOfficerDto.cs
│   │   └── ...
│   ├── Interfaces/                   # Service contracts
│   │   ├── IIncidentService.cs
│   │   ├── IAuthService.cs
│   │   ├── IEmailQueueProducer.cs
│   │   ├── ISendEmailService.cs
│   │   ├── IFileStorageService.cs
│   │   └── ...
│   ├── Validators/                   # FluentValidation validators
│   │   ├── CreateIncidentRequestValidator.cs
│   │   ├── UpdateIncidentRequestValidator.cs
│   │   └── ...
│   ├── Mappings/                     # AutoMapper profiles
│   │   └── IncidentMappingProfile.cs
│   ├── Constants/                    # Application constants
│   │   ├── IncidentStatusConstants.cs
│   │   ├── RoleConstant.cs
│   │   └── IncidentAction.cs
│   └── Repositories/
│       └── IRepository.cs            # Generic repository interface
│
├── EHS.Infrastructure/               # Infrastructure Layer
│   ├── Services/
│   │   ├── IncidentService.cs        # Main service (partial)
│   │   ├── IncidentService.Initiator.cs # Initiator operations
│   │   ├── IncidentService.SafetyOfficer.cs # Safety officer operations
│   │   ├── IncidentService.Implementor.cs # Implementor operations
│   │   ├── IncidentService.Helpers.cs # Helper methods
│   │   ├── AuthService.cs            # Authentication service
│   │   ├── AzureUserSyncService.cs   # Azure AD sync
│   │   ├── EmailService.cs           # Email service
│   │   ├── SmsService.cs             # SMS service
│   │   ├── DepartmentService.cs      # Master data service
│   │   ├── OrganizationService.cs    # Master data service
│   │   ├── MachineService.cs         # Master data service
│   │   └── ...
│   ├── Services/Email/               # Email infrastructure
│   │   ├── EmailChannel.cs           # In-memory queue
│   │   ├── InMemoryEmailQueueProducer.cs # Queue producer
│   │   ├── EmailBackgroundService.cs # Background worker
│   │   ├── AzureServiceBusEmailProducer.cs # Azure Service Bus
│   │   ├── AzureServiceBusEmailConsumer.cs # Azure Service Bus
│   │   └── AzureCommunicationEmailSender.cs # Azure Communication
│   ├── Services/Storage/             # File storage
│   │   ├── AzureBlobStorageService.cs # Azure Blob implementation
│   │   └── LocalFileStorageService.cs # Local file system
│   └── Services/Sms/                 # SMS infrastructure
│       └── ...
│
└── EHS.Domain/                       # Domain Layer
    ├── Entities/                     # Domain entities
    │   ├── Incident.cs               # Core entity
    │   ├── IncidentImplementation.cs
    │   ├── IncidentComment.cs
    │   ├── IncidentAttachment.cs
    │   ├── RootCauseAnalysisDetail.cs
    │   ├── ApplicationUser.cs        # User entity
    │   ├── Organization.cs
    │   ├── Department.cs
    │   ├── ProductionLine.cs
    │   ├── Machine.cs
    │   ├── IncidentType.cs
    │   ├── IncidentNature.cs
    │   ├── IncidentSeverity.cs
    │   ├── IncidentStatus.cs
    │   ├── ClosureAction.cs
    │   ├── Benefit.cs
    │   ├── ImplementationBenefit.cs
    │   ├── AuditLog.cs
    │   ├── AuditLogDetail.cs
    │   ├── BaseEntity.cs             # Base entity
    │   └── RefreshToken.cs
    ├── Enums/                        # Enumerations
    └── Settings/                     # Configuration models
        ├── JwtSettings.cs
        ├── EmailSettings.cs
        ├── FileStorageSettings.cs
        └── SmsSettings.cs
```

## 🔧 Key Technologies & Libraries

### Core Framework
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" />
<PackageReference Include="Swashbuckle.AspNetCore" />
```

### Database & ORM
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" />
```

### Authentication
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" />
<PackageReference Include="Microsoft.Identity.Web" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" />
```

### Email
```xml
<PackageReference Include="FluentEmail.Core" />
<PackageReference Include="FluentEmail.Razor" />
<PackageReference Include="FluentEmail.MailKit" />
```

### Validation & Mapping
```xml
<PackageReference Include="FluentValidation" />
<PackageReference Include="FluentValidation.AspNetCore" />
<PackageReference Include="AutoMapper" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" />
```

### Logging
```xml
<PackageReference Include="Serilog" />
<PackageReference Include="Serilog.AspNetCore" />
<PackageReference Include="Serilog.Sinks.Console" />
<PackageReference Include="Serilog.Sinks.File" />
```

### Azure Services
```xml
<PackageReference Include="Azure.Storage.Blobs" />
<PackageReference Include="Azure.Communication.Email" />
<PackageReference Include="Azure.Messaging.ServiceBus" />
```

## 🎨 Design Patterns Used

### 1. Repository Pattern
```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);
    Task<(IEnumerable<T>, int)> GetPagedAsync(
        int pageNumber, int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        params Expression<Func<T, object>>[] includes);
    Task AddAsync(T entity);
    void UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task SaveChangesAsync();
}
```

### 2. Service Layer Pattern
```csharp
public interface IIncidentService
{
    // Initiator operations
    Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(CreateIncidentRequest request, Guid userId);
    Task<ApiResponse<IncidentResponse>> UpdateIncidentAsync(Guid id, UpdateIncidentRequest request, Guid userId);
    
    // Safety Officer operations
    Task<ApiResponse<IncidentResponse>> RejectIncidentAsync(Guid incidentId, RejectIncidentRequest request, Guid userId);
    Task<ApiResponse<IncidentResponse>> AcceptAndAssignAsync(Guid incidentId, AcceptAndAssignRequest request, Guid userId);
    
    // Implementor operations
    Task<ApiResponse<IncidentResponse>> AcceptIncidentAsync(Guid incidentId, AcceptIncidentRequest request, Guid userId);
    Task<ApiResponse<IncidentResponse>> UpdateImplementationAsync(Guid incidentId, UpdateImplementationRequest request, Guid userId);
}
```

### 3. Dependency Injection
```csharp
// Program.cs
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<EmailChannel>();
builder.Services.AddScoped<IEmailQueueProducer, InMemoryEmailQueueProducer>();
builder.Services.AddHostedService<EmailBackgroundService>();
```

### 4. Middleware Pipeline
```csharp
// Program.cs
app.UseSerilogRequestLogging();
app.UseMiddleware<ApplicationExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>();
app.UseAuthorization();
app.MapControllers();
```

### 5. Channel Pattern (Producer-Consumer)
```csharp
public class EmailChannel
{
    private readonly Channel<EmailRequest> _channel;
    
    public EmailChannel()
    {
        _channel = Channel.CreateUnbounded<EmailRequest>();
    }
    
    public async Task WriteAsync(EmailRequest request)
    {
        await _channel.Writer.WriteAsync(request);
    }
    
    public IAsyncEnumerable<EmailRequest> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}
```

### 6. Strategy Pattern (File Storage)
```csharp
public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName);
    Task<string> GetFileUrlAsync(string fileName, string containerName, int expiryMinutes = 60);
    Task DeleteFileAsync(string fileName, string containerName);
}

// Implementations:
// - AzureBlobStorageService
// - LocalFileStorageService
// - AwsS3StorageService (future)
```

### 7. Partial Classes (Code Organization)
```csharp
// IncidentService.cs - Main class with dependencies
public partial class IncidentService : IIncidentService
{
    private readonly IRepository<Incident> _incidentRepository;
    private readonly IMapper _mapper;
    // ... other dependencies
}

// IncidentService.Initiator.cs - Initiator operations
public partial class IncidentService
{
    public async Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(...)
    {
        // Implementation
    }
}

// IncidentService.SafetyOfficer.cs - Safety Officer operations
public partial class IncidentService
{
    public async Task<ApiResponse<IncidentResponse>> RejectIncidentAsync(...)
    {
        // Implementation
    }
}
```

## 🔐 Security Implementation

### 1. Azure AD Authentication
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
```

### 2. Role-Based Authorization
```csharp
[Authorize(Roles = "Initiator,Admin")]
public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentRequest request)
{
    // Only Initiators and Admins can create incidents
}

[Authorize(Roles = "SafetyOfficer,Admin")]
public async Task<IActionResult> RejectIncident(Guid id, [FromBody] RejectIncidentRequest request)
{
    // Only Safety Officers and Admins can reject
}
```

### 3. User Context Extraction
```csharp
private Guid GetCurrentUserId()
{
    // Modified for Azure AD Hybrid Auth
    var localId = User.FindFirst("LocalUserId")?.Value;
    
    if (string.IsNullOrEmpty(localId))
    {
        throw new UnauthorizedAccessException("User context is not fully established.");
    }
    
    return Guid.Parse(localId);
}
```

### 4. Input Validation
```csharp
public class CreateIncidentRequestValidator : AbstractValidator<CreateIncidentRequest>
{
    public CreateIncidentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");
            
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");
            
        RuleFor(x => x.IncidentTypeId)
            .NotEmpty().WithMessage("Incident type is required");
            
        // ... more rules
    }
}
```

## 📧 Email System Architecture

### 1. Email Request Model
```csharp
public class EmailRequest
{
    public string ToEmail { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public object? TemplateModel { get; set; }
}
```

### 2. Email Channel (Queue)
```csharp
public class EmailChannel
{
    private readonly Channel<EmailRequest> _channel;
    
    public EmailChannel()
    {
        _channel = Channel.CreateUnbounded<EmailRequest>();
    }
    
    public async Task WriteAsync(EmailRequest request)
    {
        await _channel.Writer.WriteAsync(request);
    }
    
    public IAsyncEnumerable<EmailRequest> ReadAllAsync(CancellationToken ct)
    {
        return _channel.Reader.ReadAllAsync(ct);
    }
}
```

### 3. Background Service
```csharp
public class EmailBackgroundService : BackgroundService
{
    private readonly EmailChannel _emailChannel;
    private readonly IServiceProvider _serviceProvider;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var emailRequest in _emailChannel.ReadAllAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var fluentEmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
            
            var email = fluentEmail
                .To(emailRequest.ToEmail)
                .Subject(emailRequest.Subject)
                .UsingTemplateFromFile(
                    Path.Combine("Templates", emailRequest.TemplateName),
                    emailRequest.TemplateModel);
                    
            await email.SendAsync();
        }
    }
}
```

### 4. Razor Email Template
```cshtml
@* IncidentCreated.cshtml *@
@model dynamic

<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; }
        .container { max-width: 600px; margin: 0 auto; }
        .header { background-color: #007bff; color: white; padding: 20px; }
        .content { padding: 20px; }
        .button { background-color: #28a745; color: white; padding: 10px 20px; text-decoration: none; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h1>New Incident Reported</h1>
        </div>
        <div class="content">
            <p>A new incident has been reported:</p>
            <p><strong>Title:</strong> @Model.Title</p>
            <p><strong>Description:</strong> @Model.Description</p>
            <p><strong>Reporter:</strong> @Model.ReporterName</p>
            <p>
                <a href="@Model.ActionUrl" class="button">View Incident</a>
            </p>
        </div>
    </div>
</body>
</html>
```

## 📁 File Storage Implementation

### 1. Azure Blob Storage Service
```csharp
public class AzureBlobStorageService : IFileStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();
        
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, overwrite: true);
        
        return blobClient.Uri.ToString();
    }
    
    public async Task<string> GetFileUrlAsync(string fileName, string containerName, int expiryMinutes = 60)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var blobClient = containerClient.GetBlobClient(fileName);
        
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = containerName,
            BlobName = fileName,
            Resource = "b",
            ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(expiryMinutes)
        };
        sasBuilder.SetPermissions(BlobSasPermissions.Read);
        
        var sasUri = blobClient.GenerateSasUri(sasBuilder);
        return sasUri.ToString();
    }
}
```

### 2. File Upload Controller
```csharp
[HttpPost("upload")]
public async Task<IActionResult> UploadFile(IFormFile file)
{
    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded");
        
    // Validate file type
    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx" };
    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
    
    if (!allowedExtensions.Contains(extension))
        return BadRequest("Invalid file type");
        
    // Validate file size (max 10MB)
    if (file.Length > 10 * 1024 * 1024)
        return BadRequest("File size exceeds 10MB");
        
    // Generate unique filename
    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
    
    // Upload to Azure Blob
    using var stream = file.OpenReadStream();
    var fileUrl = await _fileStorageService.UploadFileAsync(stream, uniqueFileName, "incidents");
    
    return Ok(new FileDto
    {
        FileName = uniqueFileName,
        OriginalFileName = file.FileName,
        ContentType = file.ContentType,
        FileSize = file.Length
    });
}
```

## 🗄️ Database Context

### 1. Entity Configuration
```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public DbSet<Incident> Incidents { get; set; }
    public DbSet<IncidentImplementation> IncidentImplementations { get; set; }
    public DbSet<IncidentComment> IncidentComments { get; set; }
    public DbSet<IncidentAttachment> IncidentAttachments { get; set; }
    public DbSet<Organization> Organizations { get; set; }
    public DbSet<Department> Departments { get; set; }
    // ... other DbSets
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Incident configuration
        builder.Entity<Incident>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.InitiatedByUser)
                .WithMany()
                .HasForeignKey(e => e.InitiatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.AssignedToSafetyOfficer)
                .WithMany()
                .HasForeignKey(e => e.AssignedToSafetyOfficerId)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.Implementation)
                .WithOne(i => i.Incident)
                .HasForeignKey<IncidentImplementation>(i => i.IncidentId);
        });
        
        // Seed data
        SeedMasterData(builder);
    }
    
    private void SeedMasterData(ModelBuilder builder)
    {
        // Seed Incident Statuses
        builder.Entity<IncidentStatus>().HasData(
            new IncidentStatus { Id = Guid.NewGuid(), Name = "Submitted", Description = "Incident submitted" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "Assigned", Description = "Assigned to safety officer" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "Rejected", Description = "Rejected by safety officer" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "Approved", Description = "Approved and assigned to implementor" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "InProgress", Description = "Implementation in progress" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "VerificationPending", Description = "Pending verification" },
            new IncidentStatus { Id = Guid.NewGuid(), Name = "Closed", Description = "Incident closed" }
        );
        
        // Seed Incident Types
        builder.Entity<IncidentType>().HasData(
            new IncidentType { Id = Guid.NewGuid(), Name = "Near Miss", Description = "Near miss incident" },
            new IncidentType { Id = Guid.NewGuid(), Name = "Unsafe Condition", Description = "Unsafe condition observed" },
            new IncidentType { Id = Guid.NewGuid(), Name = "Unsafe Action", Description = "Unsafe action observed" }
        );
        
        // ... more seed data
    }
}
```

## 🧪 Testing Considerations

### 1. Unit Testing
```csharp
public class IncidentServiceTests
{
    private readonly Mock<IRepository<Incident>> _mockIncidentRepo;
    private readonly Mock<IMapper> _mockMapper;
    private readonly IncidentService _service;
    
    public IncidentServiceTests()
    {
        _mockIncidentRepo = new Mock<IRepository<Incident>>();
        _mockMapper = new Mock<IMapper>();
        _service = new IncidentService(_mockIncidentRepo.Object, _mockMapper.Object, ...);
    }
    
    [Fact]
    public async Task CreateIncident_ValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new CreateIncidentRequest { /* ... */ };
        var userId = Guid.NewGuid();
        
        // Act
        var result = await _service.CreateIncidentAsync(request, userId);
        
        // Assert
        Assert.True(result.IsSuccessful);
        Assert.NotNull(result.Data);
    }
}
```

### 2. Integration Testing
```csharp
public class IncidentsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    
    public IncidentsControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateIncident_WithValidData_Returns201()
    {
        // Arrange
        var request = new CreateIncidentRequest { /* ... */ };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/incidents", request);
        
        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
```

## 📊 Performance Optimization

### 1. Eager Loading
```csharp
private async Task<Incident?> GetIncidentWithDetailsAsync(Guid id)
{
    var incidents = await _incidentRepository.GetAllAsync(
        filter: i => i.Id == id,
        i => i.IncidentType,
        i => i.IncidentNature,
        i => i.IncidentSeverity,
        i => i.IncidentStatus,
        i => i.Organization,
        i => i.Department,
        i => i.ProductionLine!,
        i => i.Machine!,
        i => i.InitiatedByUser,
        i => i.AssignedToSafetyOfficer!,
        i => i.AssignedToImplementor!,
        i => i.Implementation!.ClosureAction,
        i => i.Implementation!.Benefits,
        i => i.Implementation!.RootCauseDetails,
        i => i.Attachments
    );
    
    return incidents.FirstOrDefault();
}
```

### 2. Pagination
```csharp
public async Task<ApiResponse<PaginatedResponse<IncidentListResponse>>> GetIncidentsAsync(
    IncidentFilterRequest filter)
{
    var (incidents, totalCount) = await _incidentRepository.GetPagedAsync(
        filter.PageNumber,
        filter.PageSize,
        filter: null,
        orderBy: q => q.OrderByDescending(i => i.CreatedAt),
        i => i.IncidentType,
        i => i.IncidentSeverity,
        i => i.IncidentStatus
    );
    
    return new ApiResponse<PaginatedResponse<IncidentListResponse>>
    {
        IsSuccessful = true,
        Data = new PaginatedResponse<IncidentListResponse>
        {
            Items = _mapper.Map<List<IncidentListResponse>>(incidents),
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            TotalCount = totalCount
        }
    };
}
```

### 3. Async/Await Throughout
```csharp
// All database operations are async
await _incidentRepository.AddAsync(incident);
await _incidentRepository.SaveChangesAsync();

// All service calls are async
await _emailService.SendEmailAsync(emailRequest);
await _fileStorageService.UploadFileAsync(stream, fileName, container);
```

## 🔍 Logging Strategy

### 1. Structured Logging with Serilog
```csharp
// Program.cs
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();
```

### 2. Service Logging
```csharp
public class IncidentService
{
    private readonly ILogger<IncidentService> _logger;
    
    public async Task<ApiResponse<IncidentResponse>> CreateIncidentAsync(...)
    {
        try
        {
            // ... create incident
            
            _logger.LogInformation("Incident created by user {UserId}", userId);
            
            return new ApiResponse<IncidentResponse> { /* ... */ };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating incident");
            return new ApiResponse<IncidentResponse>
            {
                IsSuccessful = false,
                Message = "An error occurred while creating the incident."
            };
        }
    }
}
```

### 3. Request Logging
```csharp
// Program.cs
app.UseSerilogRequestLogging();
```

## 🚀 Deployment Checklist

### 1. Configuration
- [ ] Update Azure AD settings
- [ ] Configure SMTP settings
- [ ] Set up Azure Blob Storage
- [ ] Update connection strings
- [ ] Configure logging levels

### 2. Database
- [ ] Run migrations
- [ ] Seed master data
- [ ] Create initial users and roles

### 3. Security
- [ ] Enable HTTPS
- [ ] Configure CORS
- [ ] Set up rate limiting
- [ ] Review authorization policies

### 4. Monitoring
- [ ] Set up Application Insights
- [ ] Configure health checks
- [ ] Set up alerts

---

**Generated:** 2026-01-17
**Purpose:** Technical implementation guide for developers
