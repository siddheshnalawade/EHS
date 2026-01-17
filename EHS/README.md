# EHS System - Quick Start Guide

## 📚 Documentation Index

This EHS (Environment, Health & Safety) system has comprehensive documentation split into focused guides:

1. **[PROJECT_ANALYSIS.md](./PROJECT_ANALYSIS.md)** - Complete system overview
   - Executive summary
   - Architecture overview
   - Core features
   - Domain model
   - Technology stack

2. **[WORKFLOW_DIAGRAM.md](./WORKFLOW_DIAGRAM.md)** - Visual workflows
   - Incident lifecycle flowchart
   - Status transitions
   - Role-based actions
   - Email notifications
   - Authentication flow

3. **[TECHNICAL_GUIDE.md](./TECHNICAL_GUIDE.md)** - Implementation details
   - Project structure
   - Design patterns
   - Security implementation
   - Email system
   - File storage
   - Database design

4. **[API_REFERENCE.md](./API_REFERENCE.md)** - API documentation
   - All endpoints
   - Request/response examples
   - Authentication
   - Error handling

---

## 🚀 Quick Start

### Prerequisites
- .NET 8.0 SDK or later
- SQL Server (LocalDB for development)
- Azure AD tenant (for authentication)
- Azure Storage Account (for file storage)
- SMTP server (for emails)

### 1. Clone and Restore
```bash
cd d:\Learn\DotNetProjects\EHS
dotnet restore
```

### 2. Update Configuration
Edit `EHS.API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "YOUR_CONNECTION_STRING"
  },
  "AzureAd": {
    "TenantId": "YOUR_TENANT_ID",
    "ClientId": "YOUR_CLIENT_ID",
    "Domain": "YOUR_DOMAIN.onmicrosoft.com"
  },
  "EmailSettings": {
    "SmtpHost": "YOUR_SMTP_HOST",
    "SmtpPort": 587,
    "SmtpUsername": "YOUR_USERNAME",
    "SmtpPassword": "YOUR_PASSWORD"
  },
  "FileStorage": {
    "ConnectionString": "YOUR_AZURE_STORAGE_CONNECTION"
  }
}
```

### 3. Database Setup
```bash
cd EHS.API
dotnet ef database update
```

### 4. Run the Application
```bash
dotnet run
```

The API will be available at: `https://localhost:5099`

### 5. Access Swagger UI
Navigate to: `https://localhost:5099/swagger`

---

## 🎯 System Overview

### What is this system?
An **Environment, Health & Safety (EHS) Management System** for aluminum casting companies to:
- Report safety incidents (Near Miss, Unsafe Condition, Unsafe Action)
- Track incident resolution through a structured workflow
- Assign incidents to safety officers and implementors
- Perform root cause analysis
- Document corrective actions
- Maintain audit trails for compliance

### Who uses it?
1. **Initiators** - Report incidents
2. **Safety Officers** - Review and approve incidents
3. **Implementors** - Fix incidents and document solutions
4. **Admins** - Manage system and users

### Key Features
✅ Multi-role workflow (Initiator → Safety Officer → Implementor)
✅ Azure AD authentication with local database
✅ Email notifications at each workflow stage
✅ File attachment support (Azure Blob Storage)
✅ Root Cause Analysis (RCA) tracking
✅ Benefits identification
✅ Complete audit trail
✅ RESTful API design
✅ Clean Architecture

---

## 📊 Incident Workflow Summary

```
1. INITIATOR creates incident → Status: SUBMITTED
   ↓
2. ADMIN assigns to Safety Officer → Status: ASSIGNED
   ↓
3. SAFETY OFFICER reviews:
   - Option A: Reject → Status: REJECTED (End)
   - Option B: Reassign to Initiator → Status: SUBMITTED
   - Option C: Accept & Assign to Implementor → Status: APPROVED
   ↓
4. IMPLEMENTOR accepts → Status: IN PROGRESS
   ↓
5. IMPLEMENTOR completes (RCA + Actions) → Status: VERIFICATION PENDING
   ↓
6. SAFETY OFFICER verifies & closes → Status: CLOSED
```

---

## 🔐 Authentication Setup

### Azure AD Configuration

1. **Register App in Azure AD**
   - Go to Azure Portal → Azure Active Directory → App Registrations
   - Create new registration
   - Note: Application (client) ID and Directory (tenant) ID

2. **Configure API Permissions**
   - Add Microsoft Graph permissions
   - Grant admin consent

3. **Update appsettings.json**
   ```json
   "AzureAd": {
     "Instance": "https://login.microsoftonline.com/",
     "Domain": "yourcompany.onmicrosoft.com",
     "TenantId": "your-tenant-id",
     "ClientId": "your-client-id"
   }
   ```

4. **User Sync**
   - Users are automatically synced from Azure AD to local database
   - `UserSyncMiddleware` handles this on each request
   - Local user ID is added as a claim for use in controllers

---

## 📧 Email Configuration

### SMTP Setup (Development)
Using Ethereal Email (test SMTP):
```json
"EmailSettings": {
  "SmtpHost": "smtp.ethereal.email",
  "SmtpPort": 587,
  "SmtpUsername": "your-ethereal-username",
  "SmtpPassword": "your-ethereal-password",
  "FromEmail": "no-reply@ehs.com",
  "FromName": "EHS System"
}
```

### Production SMTP
Replace with your production SMTP server (e.g., SendGrid, AWS SES, Office 365)

### Email Templates
Located in `EHS.API/Templates/`:
- `IncidentCreated.cshtml`
- `IncidentAssigned.cshtml`
- `IncidentRejected.cshtml`
- `IncidentReassigned.cshtml`

---

## 📁 File Storage Setup

### Azure Blob Storage (Production)
```json
"FileStorage": {
  "Provider": "Azure",
  "ConnectionString": "DefaultEndpointsProtocol=https;AccountName=...",
  "BasePath": "uploads"
}
```

### Local Storage (Development)
```json
"FileStorage": {
  "Provider": "Local",
  "ConnectionString": "",
  "BasePath": "C:\\Uploads"
}
```

---

## 🗄️ Database Schema

### Core Tables
- **Incidents** - Main incident records
- **IncidentImplementations** - Implementation details
- **IncidentComments** - Comments and history
- **IncidentAttachments** - File attachments
- **RootCauseAnalysisDetails** - RCA details
- **ImplementationBenefits** - Benefits identified

### Master Data Tables
- **Organizations** - Companies/plants
- **Departments** - Organizational units
- **ProductionLines** - Manufacturing lines
- **Machines** - Equipment
- **IncidentTypes** - Near Miss, Unsafe Condition, Unsafe Action
- **IncidentNatures** - New, Repeated
- **IncidentSeverities** - Minor, Major, Catastrophic
- **IncidentStatuses** - Submitted, Assigned, Approved, etc.
- **ClosureActions** - Types of closure actions
- **Benefits** - Benefit categories

### User Tables
- **AspNetUsers** - User accounts (Identity)
- **AspNetRoles** - Roles (Identity)
- **AspNetUserRoles** - User-role mapping (Identity)
- **RefreshTokens** - JWT refresh tokens

### Audit Tables
- **AuditLogs** - Audit trail
- **AuditLogDetails** - Detailed property changes

---

## 🧪 Testing the API

### Using Swagger UI
1. Navigate to `https://localhost:5099/swagger`
2. Click "Authorize" and enter your Azure AD token
3. Try the endpoints

### Using Postman
1. Import the API collection (can be generated from Swagger)
2. Set up Azure AD authentication
3. Test endpoints

### Sample Request (Create Incident)
```bash
curl -X POST "https://localhost:5099/api/v1/incidents" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Incident",
    "description": "Test description",
    "incidentTypeId": "GUID",
    "incidentNatureId": "GUID",
    "incidentSeverityId": "GUID",
    "organizationId": "GUID",
    "departmentId": "GUID",
    "incidentDate": "2026-01-17",
    "incidentArea": "Test Area",
    "proposedSolution": "Test solution"
  }'
```

---

## 🔍 Common Tasks

### Add a New User
Users are automatically created from Azure AD on first login. To assign roles:
```sql
-- Get user ID
SELECT Id, Email FROM AspNetUsers WHERE Email = 'user@company.com';

-- Get role ID
SELECT Id, Name FROM AspNetRoles WHERE Name = 'Initiator';

-- Assign role
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES ('USER_GUID', 'ROLE_GUID');
```

### Seed Master Data
Master data is seeded automatically during migration. To add more:
```sql
INSERT INTO IncidentTypes (Id, Name, Description, CreatedAt, IsDeleted)
VALUES (NEWID(), 'Equipment Failure', 'Equipment malfunction or failure', GETUTCDATE(), 0);
```

### View Incident History
```sql
SELECT 
    ic.CreatedAt,
    ic.CommentedAsRole,
    u.FullName,
    ic.Content
FROM IncidentComments ic
JOIN AspNetUsers u ON ic.CommentedByUserId = u.Id
WHERE ic.IncidentId = 'INCIDENT_GUID'
  AND ic.IsInternal = 1
ORDER BY ic.CreatedAt DESC;
```

---

## 🐛 Troubleshooting

### Issue: "User context is not fully established"
**Solution:** Ensure `UserSyncMiddleware` is configured correctly and Azure AD user exists.

### Issue: Email not sending
**Solution:** 
1. Check SMTP credentials in `appsettings.json`
2. Check logs in `Logs/` directory
3. Verify `EmailBackgroundService` is running

### Issue: File upload fails
**Solution:**
1. Check Azure Storage connection string
2. Verify container exists
3. Check file size limits (default: 10MB)

### Issue: Database connection error
**Solution:**
1. Verify connection string in `appsettings.json`
2. Ensure SQL Server is running
3. Run migrations: `dotnet ef database update`

---

## 📈 Performance Tips

1. **Use pagination** for large datasets
2. **Eager load** related entities to avoid N+1 queries
3. **Cache** master data (incident types, statuses, etc.)
4. **Use async/await** throughout
5. **Index** frequently queried columns
6. **Monitor** with Application Insights (production)

---

## 🔒 Security Checklist

- [x] Azure AD authentication
- [x] Role-based authorization
- [x] Input validation (FluentValidation)
- [x] SQL injection prevention (EF Core parameterized queries)
- [x] XSS prevention (output encoding)
- [x] HTTPS enforcement
- [x] Secure file storage (SAS tokens)
- [x] Audit logging
- [ ] Rate limiting (TODO)
- [ ] CORS configuration (TODO)

---

## 📞 Support & Contribution

### Project Structure
```
EHS/
├── EHS.API/           # Presentation layer
├── EHS.Application/   # Application logic
├── EHS.Infrastructure/# Data access & external services
└── EHS.Domain/        # Domain entities
```

### Key Files
- `Program.cs` - Application entry point
- `ServiceCollectionExtensions.cs` - DI configuration
- `IncidentService.*.cs` - Core business logic
- `ApplicationDbContext.cs` - Database context

### Logging
Logs are written to:
- Console (development)
- `Logs/log-YYYYMMDD.txt` (file)

---

## 🎓 Learning Resources

### Clean Architecture
- [Microsoft Docs - Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)

### Azure AD
- [Microsoft Identity Platform](https://docs.microsoft.com/en-us/azure/active-directory/develop/)

### Entity Framework Core
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)

### FluentEmail
- [FluentEmail GitHub](https://github.com/lukencode/FluentEmail)

---

## 📝 Next Steps

1. **Review** the documentation files
2. **Set up** your development environment
3. **Configure** Azure AD and Azure Storage
4. **Run** database migrations
5. **Test** the API using Swagger
6. **Customize** email templates
7. **Deploy** to Azure App Service (production)

---

## 🎯 Key Takeaways

✅ **Production-ready** EHS system with enterprise features
✅ **Clean Architecture** for maintainability
✅ **Azure AD integration** for SSO
✅ **Comprehensive workflow** for incident management
✅ **Email notifications** at each stage
✅ **File storage** with Azure Blob
✅ **Audit trail** for compliance
✅ **RESTful API** for frontend integration

---

**Project:** EHS System for Aluminum Casting Company
**Version:** 1.0
**Last Updated:** 2026-01-17
**Author:** Siddesh Nalawade

For questions or issues, refer to the detailed documentation files or check the code comments.
