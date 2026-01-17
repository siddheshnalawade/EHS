# EHS System - API Reference Guide

## 🔐 Authentication

All endpoints require authentication via Azure AD Bearer token, except for health check.

**Authorization Header:**
```
Authorization: Bearer {azure-ad-token}
```

**Base URL:** `https://your-domain.com/api/v1`

---

## 📋 Incident Management Endpoints

### **Initiator Operations**

#### 1. Create Incident
```http
POST /incidents
Authorization: Bearer {token}
Roles: Initiator, Admin
```

**Request Body:**
```json
{
  "title": "Unsafe condition near furnace",
  "description": "Hot metal splatter zone without proper barriers",
  "incidentTypeId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "incidentNatureId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "incidentSeverityId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "productionLineId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "machineId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "incidentDate": "2026-01-17",
  "incidentTime": "14:30:00",
  "incidentArea": "Melting Section - Furnace Area",
  "proposedSolution": "Install heat-resistant splash guards",
  "attachments": [
    {
      "fileName": "unique-guid.jpg",
      "originalFileName": "evidence.jpg",
      "contentType": "image/jpeg",
      "fileSize": 1024000
    }
  ]
}
```

**Response (201 Created):**
```json
{
  "isSuccessful": true,
  "message": "Incident created successfully.",
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Unsafe condition near furnace",
    "description": "Hot metal splatter zone without proper barriers",
    "incidentType": {
      "id": "...",
      "name": "Unsafe Condition"
    },
    "incidentStatus": {
      "id": "...",
      "name": "Submitted"
    },
    "createdAt": "2026-01-17T14:30:00Z"
  },
  "errors": []
}
```

---

#### 2. Get All Incidents (Filtered & Paginated)
```http
GET /incidents?pageNumber=1&pageSize=10
Authorization: Bearer {token}
Roles: All authenticated users
```

**Query Parameters:**
- `pageNumber` (optional, default: 1)
- `pageSize` (optional, default: 10)
- `status` (optional)
- `departmentId` (optional)
- `fromDate` (optional)
- `toDate` (optional)

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": {
    "items": [
      {
        "id": "...",
        "title": "...",
        "status": "Submitted",
        "severity": "Major",
        "createdAt": "2026-01-17T14:30:00Z"
      }
    ],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 45
  }
}
```

---

#### 3. Get Incident by ID
```http
GET /incidents/{id}
Authorization: Bearer {token}
Roles: All authenticated users
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": {
    "id": "...",
    "title": "...",
    "description": "...",
    "incidentType": { "id": "...", "name": "Near Miss" },
    "incidentNature": { "id": "...", "name": "New" },
    "incidentSeverity": { "id": "...", "name": "Major" },
    "incidentStatus": { "id": "...", "name": "Submitted" },
    "organization": { "id": "...", "name": "Taural Aluminum" },
    "department": { "id": "...", "name": "Melting" },
    "productionLine": { "id": "...", "name": "Die Casting Line 1" },
    "machine": { "id": "...", "name": "Furnace F-101" },
    "initiatedByUser": { "id": "...", "fullName": "John Doe" },
    "assignedToSafetyOfficer": null,
    "assignedToImplementor": null,
    "attachments": [
      {
        "id": "...",
        "fileName": "evidence.jpg",
        "filePath": "https://storage.blob.core.windows.net/incidents/...",
        "attachmentType": "InitialEvidence"
      }
    ],
    "implementation": null,
    "createdAt": "2026-01-17T14:30:00Z"
  }
}
```

---

#### 4. Update Incident
```http
PUT /incidents/{id}
Authorization: Bearer {token}
Roles: Initiator, Admin
```

**Request Body:** (Same as Create, but all fields optional)
```json
{
  "title": "Updated title",
  "description": "Updated description"
}
```

**Response (200 OK):** Same as Get Incident

---

#### 5. Delete Incident
```http
DELETE /incidents/{id}
Authorization: Bearer {token}
Roles: Initiator, Admin
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "message": "Incident deleted successfully."
}
```

---

#### 6. Get My Incidents
```http
GET /incidents/my-incidents?pageNumber=1&pageSize=10
Authorization: Bearer {token}
Roles: Initiator, Admin
```

**Response (200 OK):** Paginated list of user's incidents

---

### **Safety Officer Operations**

#### 7. Get Pending Reviews
```http
GET /incidents/pending-review?pageNumber=1&pageSize=10
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Response (200 OK):** Paginated list of incidents pending review

---

#### 8. Get Pending Verifications
```http
GET /incidents/pending-verification?pageNumber=1&pageSize=10
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Response (200 OK):** Paginated list of incidents pending verification

---

#### 9. Reject Incident
```http
POST /incidents/{id}/reject
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Request Body:**
```json
{
  "comment": "Insufficient evidence provided. Please add photos of the incident area."
}
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "message": "Incident rejected successfully.",
  "data": {
    "id": "...",
    "status": "Rejected",
    "reviewerComment": "Insufficient evidence provided..."
  }
}
```

---

#### 10. Reassign to Initiator
```http
POST /incidents/{id}/reassign-to-initiator
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Request Body:**
```json
{
  "comment": "Please update the proposed solution with more details."
}
```

**Response (200 OK):** Updated incident with status "Submitted"

---

#### 11. Accept and Assign to Implementor
```http
POST /incidents/{id}/accept-and-assign
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Request Body:**
```json
{
  "implementorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "comment": "Approved. Please implement the proposed solution."
}
```

**Response (200 OK):** Updated incident with status "Approved"

---

#### 12. Close Incident
```http
POST /incidents/{id}/close
Authorization: Bearer {token}
Roles: SafetyOfficer, Admin
```

**Request Body:**
```json
{
  "closureComment": "Implementation verified. Incident closed successfully."
}
```

**Response (200 OK):** Updated incident with status "Closed"

---

### **Implementor Operations**

#### 13. Get My Implementations
```http
GET /incidents/my-implementations?pageNumber=1&pageSize=10
Authorization: Bearer {token}
Roles: Implementor, Admin
```

**Response (200 OK):** Paginated list of assigned incidents

---

#### 14. Accept Incident
```http
POST /incidents/{id}/accept
Authorization: Bearer {token}
Roles: Implementor, Admin
```

**Request Body:**
```json
{
  "estimatedDaysToComplete": 7
}
```

**Response (200 OK):** Updated incident with status "InProgress"

---

#### 15. Pass to Peer
```http
POST /incidents/{id}/pass-to-peer
Authorization: Bearer {token}
Roles: Implementor, Admin
```

**Request Body:**
```json
{
  "peerImplementorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "comment": "This requires expertise in electrical systems. Passing to John."
}
```

**Response (200 OK):** Updated incident with new implementor

---

#### 16. Update Implementation
```http
PUT /incidents/{id}/implementation
Authorization: Bearer {token}
Roles: Implementor, Admin
```

**Request Body:**
```json
{
  "rootCauseAnalysis": "Lack of proper barriers in high-risk area",
  "correctiveActionsDescription": "Installed heat-resistant splash guards and warning signs",
  "closureActionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "additionalRemarks": "Additional training provided to operators",
  "rootCauseDetails": [
    {
      "rootCause": "Inadequate safety barriers",
      "correctiveMeasure": "Install splash guards",
      "responsibleParty": "Maintenance Team",
      "targetCompletionDate": "2026-01-24",
      "status": "Completed"
    }
  ],
  "benefitIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "3fa85f64-5717-4562-b3fc-2c963f66afa7"
  ],
  "attachments": [
    {
      "fileName": "after-photo.jpg",
      "originalFileName": "implementation-evidence.jpg",
      "contentType": "image/jpeg",
      "fileSize": 2048000
    }
  ],
  "markAsCompleted": true
}
```

**Response (200 OK):** Updated incident with implementation details and status "VerificationPending"

---

### **Admin Operations**

#### 17. Assign to Safety Officer
```http
POST /incidents/{id}/assign-to-safety-officer
Authorization: Bearer {token}
Roles: Admin
```

**Request Body:**
```json
{
  "safetyOfficerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Response (200 OK):** Updated incident with status "Assigned"

---

## 📁 File Storage Endpoints

### 18. Upload File
```http
POST /storage/upload
Authorization: Bearer {token}
Content-Type: multipart/form-data
```

**Request Body:**
```
file: [binary file data]
```

**Response (200 OK):**
```json
{
  "fileName": "3fa85f64-5717-4562-b3fc-2c963f66afa6.jpg",
  "originalFileName": "evidence.jpg",
  "contentType": "image/jpeg",
  "fileSize": 1024000,
  "url": "https://storage.blob.core.windows.net/incidents/..."
}
```

---

### 19. Get File URL
```http
GET /storage/{fileName}?containerName=incidents
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "url": "https://storage.blob.core.windows.net/incidents/...?sv=2021-06-08&se=..."
}
```

---

### 20. Delete File
```http
DELETE /storage/{fileName}?containerName=incidents
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "message": "File deleted successfully."
}
```

---

## 🏢 Master Data Endpoints

### Organizations

#### 21. Get All Organizations
```http
GET /organizations?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### 22. Get Organization by ID
```http
GET /organizations/{id}
Authorization: Bearer {token}
```

#### 23. Create Organization
```http
POST /organizations
Authorization: Bearer {token}
Roles: Admin
```

#### 24. Update Organization
```http
PUT /organizations/{id}
Authorization: Bearer {token}
Roles: Admin
```

#### 25. Delete Organization
```http
DELETE /organizations/{id}
Authorization: Bearer {token}
Roles: Admin
```

---

### Departments

#### 26. Get All Departments
```http
GET /departments?pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### 27. Get Department by ID
```http
GET /departments/{id}
Authorization: Bearer {token}
```

#### 28. Create Department
```http
POST /departments
Authorization: Bearer {token}
Roles: Admin
```

**Request Body:**
```json
{
  "name": "Melting Department",
  "code": "MELT",
  "description": "Aluminum melting and furnace operations",
  "organizationId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "headOfDepartment": "John Doe",
  "contactEmail": "melting@company.com",
  "contactPhone": "+1234567890"
}
```

---

### Production Lines

#### 29. Get All Production Lines
```http
GET /production-lines?departmentId={id}&pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### 30. Create Production Line
```http
POST /production-lines
Authorization: Bearer {token}
Roles: Admin
```

**Request Body:**
```json
{
  "name": "Die Casting Line 1",
  "code": "DCL-1",
  "description": "High-pressure die casting line",
  "departmentId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "capacity": "500 units/day",
  "status": "Active"
}
```

---

### Machines

#### 31. Get All Machines
```http
GET /machines?productionLineId={id}&pageNumber=1&pageSize=10
Authorization: Bearer {token}
```

#### 32. Create Machine
```http
POST /machines
Authorization: Bearer {token}
Roles: Admin
```

**Request Body:**
```json
{
  "name": "Furnace F-101",
  "code": "F-101",
  "description": "Reverberatory furnace for aluminum melting",
  "productionLineId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "manufacturer": "ABC Furnaces Inc.",
  "model": "RF-500",
  "serialNumber": "SN123456",
  "installationDate": "2020-01-15",
  "status": "Operational"
}
```

---

### Incident Types

#### 33. Get All Incident Types
```http
GET /incident-types
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": [
    {
      "id": "...",
      "name": "Near Miss",
      "description": "An incident that could have resulted in injury or damage"
    },
    {
      "id": "...",
      "name": "Unsafe Condition",
      "description": "A condition that poses a safety risk"
    },
    {
      "id": "...",
      "name": "Unsafe Action",
      "description": "An action that violates safety procedures"
    }
  ]
}
```

---

### Incident Natures

#### 34. Get All Incident Natures
```http
GET /incident-natures
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": [
    {
      "id": "...",
      "name": "New",
      "description": "First occurrence of this type of incident"
    },
    {
      "id": "...",
      "name": "Repeated",
      "description": "Similar incident has occurred before"
    }
  ]
}
```

---

### Incident Severities

#### 35. Get All Incident Severities
```http
GET /incident-severities
Authorization: Bearer {token}
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": [
    {
      "id": "...",
      "name": "Minor",
      "description": "Low impact, easily correctable",
      "level": 1
    },
    {
      "id": "...",
      "name": "Major",
      "description": "Significant impact, requires immediate attention",
      "level": 2
    },
    {
      "id": "...",
      "name": "Catastrophic",
      "description": "Severe impact, potential for serious injury or damage",
      "level": 3
    }
  ]
}
```

---

## 🔐 Authentication Endpoints

### 36. Login (Custom JWT - if not using Azure AD)
```http
POST /auth/login
```

**Request Body:**
```json
{
  "email": "user@company.com",
  "password": "SecurePassword123!"
}
```

**Response (200 OK):**
```json
{
  "isSuccessful": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "...",
    "expiresAt": "2026-01-18T14:30:00Z",
    "user": {
      "id": "...",
      "fullName": "John Doe",
      "email": "user@company.com",
      "roles": ["Initiator"]
    }
  }
}
```

---

### 37. Refresh Token
```http
POST /auth/refresh-token
```

**Request Body:**
```json
{
  "refreshToken": "..."
}
```

---

## 🏥 Health Check

### 38. Health Check
```http
GET /health
```

**Response (200 OK):**
```json
{
  "status": "Healthy",
  "timestamp": "2026-01-17T14:30:00Z"
}
```

---

## ❌ Error Responses

### Validation Error (400 Bad Request)
```json
{
  "isSuccessful": false,
  "message": "Validation failed",
  "errors": [
    {
      "field": "Title",
      "message": "Title is required"
    },
    {
      "field": "Description",
      "message": "Description cannot exceed 2000 characters"
    }
  ]
}
```

### Unauthorized (401)
```json
{
  "isSuccessful": false,
  "message": "Unauthorized. Please provide a valid token."
}
```

### Forbidden (403)
```json
{
  "isSuccessful": false,
  "message": "You do not have permission to perform this action."
}
```

### Not Found (404)
```json
{
  "isSuccessful": false,
  "message": "Incident not found."
}
```

### Internal Server Error (500)
```json
{
  "isSuccessful": false,
  "message": "An error occurred while processing your request."
}
```

---

## 📝 Notes

1. **All dates** are in ISO 8601 format (UTC): `2026-01-17T14:30:00Z`
2. **All IDs** are GUIDs: `3fa85f64-5717-4562-b3fc-2c963f66afa6`
3. **Pagination** defaults: pageNumber=1, pageSize=10
4. **File uploads** must be done separately before creating/updating incidents
5. **SAS URLs** for file access expire after 60 minutes
6. **Soft deletes** are used; deleted records are marked as `IsDeleted=true`

---

**Generated:** 2026-01-17
**Purpose:** Quick API reference for developers and frontend teams
