# EHS System - Comprehensive Project Analysis

## 📋 Executive Summary

This is an **Environment, Health & Safety (EHS) Management System** designed specifically for an **aluminum casting company**. The system enables comprehensive incident reporting, tracking, and resolution through a structured workflow involving multiple stakeholders.

**Technology Stack:**
- **Backend:** ASP.NET Core (.NET 8+) Web API
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API layers)
- **Database:** SQL Server (LocalDB for development)
- **Authentication:** Hybrid Azure AD + Local Database
- **Email:** FluentEmail with MailKit + Razor templates
- **File Storage:** Azure Blob Storage
- **Logging:** Serilog
- **Mapping:** AutoMapper
- **Validation:** FluentValidation

---

## 🏗️ System Architecture

### Clean Architecture Layers

```
┌─────────────────────────────────────────────────────────┐
│                      EHS.API                            │
│  (Controllers, Middlewares, Program.cs)                 │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                  EHS.Application                        │
│  (DTOs, Interfaces, Validators, Mappings)               │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                 EHS.Infrastructure                      │
│  (Services, Repositories, Email, Storage)               │
└─────────────────────────────────────────────────────────┘
                           ↓
┌─────────────────────────────────────────────────────────┐
│                    EHS.Domain                           │
│  (Entities, Enums, Settings)                            │
└─────────────────────────────────────────────────────────┘
```

---

## 🎯 Core Features

### 1. **Incident Management Workflow**

The system implements a sophisticated multi-stage incident workflow:

```
┌──────────────┐
│   INITIATOR  │ Creates incident with evidence
└──────┬───────┘
       │
       ↓
┌──────────────┐
│    ADMIN     │ Assigns to Safety Officer
└──────┬───────┘
       │
       ↓
┌──────────────────┐
│ SAFETY OFFICER   │ Reviews & decides:
└──────┬───────────┘  • Reject
       │              • Reassign to Initiator
       │              • Accept & Assign to Implementor
       ↓
┌──────────────────┐
│  IMPLEMENTOR     │ Implements corrective actions:
└──────┬───────────┘  • Accept incident
       │              • Pass to peer
       │              • Complete with RCA & benefits
       ↓
┌──────────────────┐
│ SAFETY OFFICER   │ Verifies & Closes
└──────────────────┘
```

### 2. **Role-Based Access Control**

**Four Primary Roles:**

1. **Initiator** - Reports incidents
   - Create incidents with attachments
   - View own incidents
   - Update/delete incidents (only in "Submitted" status)

2. **Safety Officer** - Reviews and manages incidents
   - View pending reviews
   - Reject incidents with comments
   - Reassign to initiator for modifications
   - Accept and assign to implementor
   - Close incidents after verification

3. **Implementor** - Fixes incidents
   - View assigned incidents
   - Accept incidents with estimated timeline
   - Pass to peer implementors
   - Complete implementation with:
     - Root Cause Analysis (RCA)
     - Corrective actions
     - Benefits identification
     - Evidence attachments

4. **Admin** - System administration
   - Assign incidents to safety officers
   - Full system access

---

## 📊 Domain Model

### Core Entities

#### **Incident** (Central Entity)
```csharp
- Id, Title, Description, IncidentArea
- IncidentDate, IncidentTime
- IncidentType (Near Miss, Unsafe Condition, Unsafe Action)
- IncidentNature (New, Repeated)
- IncidentSeverity (Minor, Major, Catastrophic)
- IncidentStatus (Submitted, Assigned, Approved, InProgress, etc.)
- Organization, Department, ProductionLine, Machine
- InitiatedByUser, AssignedToSafetyOfficer, AssignedToImplementor
- ProposedSolution, ReviewerComment
- Timestamps: AssignedAt, ApprovedAt, RejectedAt, ClosedAt
- Navigation: Comments, Implementation, Attachments, AuditLogs
```

#### **IncidentImplementation** (One-to-One with Incident)
```csharp
- EstimatedDaysToComplete
- AcceptedAt, StartedAt, CompletedAt
- RootCauseAnalysis, CorrectiveActionsDescription
- ClosureAction, AdditionalRemarks
- Status (Pending, InProgress, Completed, VerificationPending)
- Navigation: Benefits, RootCauseDetails
```

#### **RootCauseAnalysisDetail** (Multiple per Implementation)
```csharp
- RootCause, CorrectiveMeasure
- ResponsibleParty
- TargetCompletionDate, ActualCompletionDate
- Status (NotStarted, InProgress, Completed, Verified)
```

#### **IncidentComment** (Audit Trail & Communication)
```csharp
- Content, CommentedByUser, CommentedAsRole
- IsInternal (for system history vs user comments)
```

#### **IncidentAttachment** (File Management)
```csharp
- FileName, FilePath (Azure Blob URL)
- AttachmentType (InitialEvidence, ImplementorEvidence, RCA)
- ContentType, FileSize
- UploadedByUser
```

#### **Supporting Entities**
- **Organization** - Company/plant details
- **Department** - Organizational units
- **ProductionLine** - Manufacturing lines
- **Machine** - Equipment involved
- **IncidentType, IncidentNature, IncidentSeverity** - Master data
- **ClosureAction** - Types of closure actions
- **Benefit** - Benefits from implementations
- **AuditLog & AuditLogDetail** - Complete audit trail

---

## 🔐 Authentication & Authorization

### Hybrid Azure AD Authentication

The system uses a **hybrid authentication model**:

1. **Azure AD** handles authentication (SSO)
2. **Local Database** stores user profiles and roles

**Flow:**
```
User Login → Azure AD Token → UserSyncMiddleware → 
Sync Azure User to Local DB → Add LocalUserId Claim → 
Controllers use LocalUserId for operations
```

**Key Components:**
- `AzureUserSyncService` - Syncs Azure AD users to local database
- `UserSyncMiddleware` - Adds LocalUserId claim to authenticated users
- `ApplicationUser.AzureObjectId` - Links local user to Azure AD

**Configuration (appsettings.json):**
```json
"AzureAd": {
  "Instance": "https://login.microsoftonline.com/",
  "Domain": "YOUR_DOMAIN.onmicrosoft.com",
  "TenantId": "YOUR_TENANT_ID",
  "ClientId": "YOUR_CLIENT_ID"
}
```

---

## 📧 Email Notification System

### Architecture

**Channel-Based Background Processing:**
```
Service → EmailChannel (In-Memory Queue) → 
EmailBackgroundService → FluentEmail + MailKit → SMTP
```

**Email Templates (Razor):**
1. `IncidentCreated.cshtml` - New incident notification
2. `IncidentAssigned.cshtml` - Assignment notification
3. `IncidentRejected.cshtml` - Rejection notification
4. `IncidentReassigned.cshtml` - Reassignment notification

**Notification Triggers:**
- Incident creation → Admin
- Assignment to Safety Officer → Safety Officer + Initiator
- Rejection → Initiator
- Reassignment → Initiator
- (Implementation completion notifications can be added)

**Configuration:**
```json
"EmailSettings": {
  "SmtpHost": "smtp.ethereal.email",
  "SmtpPort": 587,
  "SmtpUsername": "clinton20@ethereal.email",
  "FromEmail": "no-reply@ehs.com",
  "FromName": "EHS System"
}
```

---

## 📁 File Storage System

### Azure Blob Storage Integration

**Features:**
- Upload files to Azure Blob Storage
- Generate temporary SAS URLs (60-minute validity)
- Support for multiple attachment types
- File validation (size, type)

**Attachment Types:**
1. **InitialEvidence** - Uploaded by initiator during incident creation
2. **ImplementorEvidence** - Uploaded by implementor during implementation
3. **RCA** - Root cause analysis documents

**Configuration:**
```json
"FileStorage": {
  "Provider": "Azure",
  "ConnectionString": "UseDevelopmentStorage=true",
  "BasePath": "uploads"
}
```

---

## 🔍 Service Layer Architecture

### Partial Classes Pattern

The `IncidentService` is split into focused partial classes:

1. **IncidentService.cs** - Main class with dependencies
2. **IncidentService.Initiator.cs** - Initiator operations
   - CreateIncident, GetIncidents, GetIncidentById
   - UpdateIncident, DeleteIncident, GetMyIncidents

3. **IncidentService.SafetyOfficer.cs** - Safety Officer operations
   - AssignToSafetyOfficer, RejectIncident
   - ReassignToInitiator, AcceptAndAssign
   - GetPendingReview, GetPendingVerification, CloseIncident

4. **IncidentService.Implementor.cs** - Implementor operations
   - AcceptIncident, PassToPeer
   - UpdateImplementation, GetMyImplementations

5. **IncidentService.Helpers.cs** - Shared utilities
   - AddIncidentHistoryAsync (creates internal comments for audit)
   - GetStatusByNameAsync
   - GetIncidentWithDetailsAsync (eager loading with SAS URLs)

---

## 🎨 API Design

### RESTful Endpoints

**Base URL:** `/api/v1/incidents`

#### Initiator Endpoints
```
POST   /api/v1/incidents                    - Create incident
GET    /api/v1/incidents                    - Get all incidents (filtered)
GET    /api/v1/incidents/{id}               - Get incident by ID
PUT    /api/v1/incidents/{id}               - Update incident
DELETE /api/v1/incidents/{id}               - Delete incident
GET    /api/v1/incidents/my-incidents       - Get user's incidents
```

#### Safety Officer Endpoints
```
GET    /api/v1/incidents/pending-review           - Get pending reviews
GET    /api/v1/incidents/pending-verification     - Get pending verifications
POST   /api/v1/incidents/{id}/reject              - Reject incident
POST   /api/v1/incidents/{id}/reassign-to-initiator - Reassign to initiator
POST   /api/v1/incidents/{id}/accept-and-assign   - Accept & assign to implementor
POST   /api/v1/incidents/{id}/close               - Close incident
```

#### Implementor Endpoints
```
GET    /api/v1/incidents/my-implementations       - Get assigned incidents
POST   /api/v1/incidents/{id}/accept              - Accept incident
POST   /api/v1/incidents/{id}/pass-to-peer        - Pass to peer
PUT    /api/v1/incidents/{id}/implementation      - Update implementation
```

#### Admin Endpoints
```
POST   /api/v1/incidents/{id}/assign-to-safety-officer - Assign to safety officer
```

### Response Format

**Standard API Response:**
```json
{
  "isSuccessful": true,
  "message": "Incident created successfully.",
  "data": { /* IncidentResponse */ },
  "errors": []
}
```

**Paginated Response:**
```json
{
  "isSuccessful": true,
  "data": {
    "items": [ /* array of items */ ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 45
  }
}
```

---

## 🔄 Incident Status Workflow

### Status Constants
```csharp
- Submitted           // Initial state
- Assigned            // Assigned to Safety Officer
- Rejected            // Rejected by Safety Officer
- Approved            // Approved and assigned to Implementor
- InProgress          // Implementor working on it
- VerificationPending // Implementation completed, awaiting verification
- Closed              // Verified and closed
```

### State Transitions
```
Submitted → Assigned → Approved → InProgress → VerificationPending → Closed
    ↓           ↓
 Rejected   Submitted (reassigned)
```

---

## 📝 Audit Trail & History

### Two-Level Tracking

1. **IncidentComment (Internal)**
   - System-generated history entries
   - `IsInternal = true`
   - Tracks all state changes with role and action

2. **AuditLog**
   - Comprehensive audit trail
   - Tracks entity changes with before/after values
   - Includes IP address, user agent, timestamp

**Actions Tracked:**
```csharp
- Created
- AssignedToSafetyOfficer
- SafetyOfficerRejected
- AssignedToInitiator
- AssignedToImplementor
- ImplementorAccepted
- ImplementationCompleted
- IncidentClosed
```

---

## 🛡️ Validation & Error Handling

### FluentValidation

Each request DTO has a dedicated validator:
- `CreateIncidentRequestValidator`
- `UpdateIncidentRequestValidator`
- `RejectIncidentRequestValidator`
- `AcceptAndAssignRequestValidator`
- `UpdateImplementationRequestValidator`
- etc.

### Global Exception Handling

**ApplicationExceptionHandlingMiddleware:**
- Catches all exceptions
- Returns consistent error responses
- Logs errors with Serilog

---

## 📊 Logging Strategy

### Serilog Configuration

**Sinks:**
1. Console - Development debugging
2. File - Rolling daily logs in `Logs/` directory

**Log Levels:**
- Debug - Development
- Information - Key operations (incident created, assigned, etc.)
- Error - Exceptions and failures

**Enrichers:**
- FromLogContext
- WithMachineName
- WithProcessId
- WithThreadId

---

## 🗄️ Database Design Highlights

### Key Relationships

```
Organization 1:N Department 1:N ProductionLine 1:N Machine

Incident N:1 IncidentType
Incident N:1 IncidentNature
Incident N:1 IncidentSeverity
Incident N:1 IncidentStatus
Incident N:1 Organization
Incident N:1 Department
Incident N:1 ProductionLine (optional)
Incident N:1 Machine (optional)
Incident N:1 InitiatedByUser
Incident N:1 AssignedToSafetyOfficer (optional)
Incident N:1 AssignedToImplementor (optional)
Incident 1:1 IncidentImplementation
Incident 1:N IncidentComment
Incident 1:N IncidentAttachment
Incident 1:N AuditLog

IncidentImplementation 1:N RootCauseAnalysisDetail
IncidentImplementation 1:N ImplementationBenefit N:1 Benefit
IncidentImplementation N:1 ClosureAction
```

### BaseEntity Pattern

All entities inherit from `BaseEntity`:
```csharp
- Id (Guid)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- IsDeleted (bool) - Soft delete support
```

---

## 🚀 Deployment Configuration

### Connection String
```
Data Source=(localdb)\\MSSQLLocalDB;
Initial Catalog=TauralEHS;
Integrated Security=True;
```

### Environment-Specific Settings
- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development overrides

---

## 🔧 Key Design Patterns

1. **Repository Pattern** - Data access abstraction
2. **Unit of Work** - Transaction management
3. **Dependency Injection** - Loose coupling
4. **Clean Architecture** - Separation of concerns
5. **CQRS-lite** - Read/write separation in services
6. **Partial Classes** - Service organization
7. **Channel Pattern** - Background email processing
8. **Strategy Pattern** - File storage providers

---

## 📈 Future Enhancements (Based on TODOs)

1. **Rate Limiting** - API throttling
2. **CORS Policy** - Frontend integration
3. **Swagger Documentation** - Already configured (OpenAPI)
4. **SMS Notifications** - Twilio integration (infrastructure ready)
5. **Azure Service Bus** - For email/SMS queuing (configured but not active)
6. **Advanced Filtering** - Incident search by date, status, department, etc.
7. **Dashboard & Analytics** - Incident trends, KPIs
8. **Mobile App Support** - API-first design supports it
9. **Multi-language Support** - Internationalization
10. **Advanced Reporting** - PDF/Excel exports

---

## 🎓 Best Practices Implemented

✅ Clean Architecture with clear layer separation
✅ SOLID principles
✅ Async/await throughout
✅ Comprehensive error handling
✅ Structured logging
✅ Input validation with FluentValidation
✅ AutoMapper for DTO mapping
✅ Role-based authorization
✅ Audit trail for compliance
✅ Soft delete for data retention
✅ Pagination for large datasets
✅ Background processing for emails
✅ SAS URLs for secure file access
✅ Hybrid authentication for flexibility

---

## 🏭 Aluminum Casting Industry Context

The system is tailored for aluminum casting operations with:

- **Production Lines** - Casting lines (e.g., Die Casting, Sand Casting)
- **Machines** - Furnaces, molding machines, CNC equipment
- **Departments** - Melting, Casting, Machining, Quality, Maintenance
- **Incident Types** - Near Miss, Unsafe Condition, Unsafe Action
- **Severity Levels** - Minor, Major, Catastrophic
- **Safety Focus** - Proactive incident reporting and resolution

---

## 📞 Support & Maintenance

### Code Organization
- Well-commented code
- XML documentation on public APIs
- Consistent naming conventions
- Logical file structure

### Extensibility
- Interface-based design for easy mocking/testing
- Provider pattern for storage (can switch from Azure to AWS)
- Configurable email providers
- Pluggable authentication

---

## 🎯 Summary

This is a **production-ready, enterprise-grade EHS system** with:
- ✅ Complete incident lifecycle management
- ✅ Multi-role workflow
- ✅ Azure AD integration
- ✅ Email notifications
- ✅ File management
- ✅ Comprehensive audit trail
- ✅ Clean, maintainable architecture

The system demonstrates advanced .NET development practices and is well-suited for safety-critical manufacturing environments like aluminum casting facilities.

---

**Generated:** 2026-01-17
**Project:** EHS System for Aluminum Casting Company
**Architecture:** Clean Architecture with ASP.NET Core
