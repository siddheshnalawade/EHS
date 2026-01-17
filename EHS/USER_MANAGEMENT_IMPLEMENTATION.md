# User Management Implementation Summary

## ✅ What I Fixed

### 1. **UserSyncMiddleware.cs** - Critical Bug Fixes
- ❌ **Before:** Missing `user` variable - code would not compile
- ✅ **After:** Properly calls `syncService.SyncUserAsync()` to get user
- ✅ Added error handling with proper HTTP status codes
- ✅ Added role claims so `[Authorize(Roles="Initiator")]` works
- ✅ Added user info claims (FullName, Email)
- ✅ Added active user check

### 2. **AzureUserSyncService.cs** - Enhanced Functionality
- ✅ Added `UpdateUserProfileAsync()` - syncs name/email changes from Azure
- ✅ Added `CreateJITUserAsync()` - cleaner user creation
- ✅ Added `AssignDefaultRoleAsync()` - smart role assignment
- ✅ Better logging with structured logging
- ✅ Updates `LastLoginAt` timestamp

---

## 🎯 How It Works Now

### User Login Flow

```
1. User logs in → Azure Entra ID validates
   ↓
2. Azure issues JWT token with claims (OID, email, name)
   ↓
3. User accesses EHS API → Token sent in Authorization header
   ↓
4. ASP.NET Core validates token signature
   ↓
5. UserSyncMiddleware runs:
   ├─ Extracts Azure claims (OID, email, name)
   ├─ Checks local database:
   │  ├─ User exists by AzureObjectId → Update profile, return user
   │  ├─ User exists by Email (no OID) → Link to Azure, return user
   │  └─ User doesn't exist → Create new user (JIT)
   ├─ Checks if user is active
   ├─ Adds LocalUserId claim
   ├─ Adds role claims from local database
   └─ Adds user info claims
   ↓
6. Controller receives request with all claims
   ↓
7. GetCurrentUserId() extracts LocalUserId claim
   ↓
8. Business logic uses local Guid for database operations
```

---

## 📋 Default Role Assignment Logic

The system now intelligently assigns roles based on email domain:

```csharp
if (email.EndsWith("@admin.company.com"))
    → Assign "Admin" role

else if (email.EndsWith("@safety.company.com"))
    → Assign "SafetyOfficer" role

else
    → Assign "Initiator" role (default)
```

**You can customize this in `AzureUserSyncService.AssignDefaultRoleAsync()`**

---

## 🔄 User Scenarios

### Scenario 1: New Employee (First Login)
```
1. HR creates user in Azure Entra ID
2. User logs into EHS app
3. JIT provisioning creates local user
4. Default role assigned (Initiator)
5. Admin can later change role to SafetyOfficer/Implementor
```

### Scenario 2: Existing User (Migration)
```
1. User already exists in local DB (created manually)
2. User logs in with Azure for first time
3. System links Azure OID to existing user
4. User keeps existing roles and data
```

### Scenario 3: Profile Update
```
1. User changes name in Azure AD
2. User logs into EHS app
3. System detects name change
4. Updates local database automatically
```

### Scenario 4: User Leaves Company
```
1. HR disables user in Azure Entra ID
2. User cannot login (Azure blocks)
3. Admin can mark user as inactive in local DB
4. Historical data preserved
```

---

## 🛠️ Configuration Required

### 1. Update appsettings.json

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "yourcompany.onmicrosoft.com",
    "TenantId": "your-tenant-id",
    "ClientId": "your-client-id",
    "CallbackPath": "/signin-oidc",
    "Scopes": "access_as_user"
  }
}
```

### 2. Ensure Roles Exist in Database

Run this SQL to create default roles:

```sql
-- Check if roles exist
SELECT * FROM AspNetRoles;

-- If not, create them
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (NEWID(), 'Admin', 'ADMIN', NEWID()),
    (NEWID(), 'SafetyOfficer', 'SAFETYOFFICER', NEWID()),
    (NEWID(), 'Implementor', 'IMPLEMENTOR', NEWID()),
    (NEWID(), 'Initiator', 'INITIATOR', NEWID());
```

### 3. Middleware Registration (Already Done)

In `Program.cs`:
```csharp
app.UseAuthentication();
app.UseMiddleware<UserSyncMiddleware>(); // ✅ Already configured
app.UseAuthorization();
```

---

## 🎨 Customization Options

### Option 1: Change Default Role

Edit `AzureUserSyncService.AssignDefaultRoleAsync()`:

```csharp
// Instead of Initiator, assign User role
await _userManager.AddToRoleAsync(user, "User");
```

### Option 2: Assign Based on Department

```csharp
var department = principal.FindFirstValue("department");

if (department == "Safety")
    await _userManager.AddToRoleAsync(user, "SafetyOfficer");
else if (department == "Maintenance")
    await _userManager.AddToRoleAsync(user, "Implementor");
else
    await _userManager.AddToRoleAsync(user, "Initiator");
```

### Option 3: Disable JIT Provisioning

If you want to manually create all users:

```csharp
// In AzureUserSyncService.SyncUserAsync()
// Comment out or remove this section:
// return await CreateJITUserAsync(oid, email, name);

// Instead, return null:
_logger.LogWarning("User {Email} not found in local database", email);
return null;
```

Then users must be pre-created by admin.

---

## 📊 For Multi-Application (EHS, CMMS, QC)

### Shared User Database

All three applications can use the **same user database**:

```
┌─────────────────────────────────────┐
│     Shared User Database            │
│  (Single SQL Server Database)       │
├─────────────────────────────────────┤
│  ApplicationUser                    │
│  ├─ AzureObjectId                   │
│  ├─ Email, FullName                 │
│  ├─ Roles (Admin, SafetyOfficer...) │
│  └─ App Access Flags (future)       │
└─────────────────────────────────────┘
         ↓           ↓           ↓
    ┌────────┐  ┌────────┐  ┌────────┐
    │  EHS   │  │  CMMS  │  │   QC   │
    └────────┘  └────────┘  └────────┘
```

### Connection String

All three apps point to same database:

```json
// appsettings.json (same for EHS, CMMS, QC)
{
  "ConnectionStrings": {
    "Default": "Server=your-server;Database=CompanyUsers;..."
  }
}
```

### Future Enhancement: App-Specific Access

Add to `ApplicationUser`:

```csharp
public bool HasEHSAccess { get; set; } = false;
public bool HasCMMSAccess { get; set; } = false;
public bool HasQCAccess { get; set; } = false;
```

Then check in middleware:

```csharp
// In EHS app
if (!user.HasEHSAccess)
{
    return Forbid("You don't have access to EHS application");
}
```

---

## 🧪 Testing

### Test 1: New User Login
1. Create user in Azure Entra ID
2. Login to EHS app
3. Check database - user should be created
4. Check roles - should have Initiator role

### Test 2: Existing User Login
1. Manually create user in database (no AzureObjectId)
2. Login with Azure
3. Check database - AzureObjectId should be populated

### Test 3: Profile Update
1. Change name in Azure AD
2. Login to EHS app
3. Check database - FullName should be updated

### Test 4: Role Authorization
1. Login as Initiator
2. Try to access SafetyOfficer endpoint
3. Should get 403 Forbidden

---

## 📝 Next Steps

1. ✅ Code is fixed and ready to use
2. ⏭️ Test JIT provisioning with a new user
3. ⏭️ Customize default role assignment logic
4. ⏭️ Create admin UI for user management (optional)
5. ⏭️ Set up CMMS and QC apps with same approach
6. ⏭️ Add app-specific access control (if needed)

---

## 🎓 Key Takeaways

✅ **JIT Provisioning** - Users auto-created on first login
✅ **Profile Sync** - Name/email updated from Azure automatically
✅ **Role Claims** - Local roles work with `[Authorize(Roles="...")]`
✅ **Error Handling** - Proper HTTP status codes and messages
✅ **Logging** - Structured logging for debugging
✅ **Multi-App Ready** - Same approach works for EHS, CMMS, QC

---

**Generated:** 2026-01-17
**Status:** ✅ Implementation Complete
**Files Modified:**
- `EHS.API/Middlewares/UserSyncMiddleware.cs`
- `EHS.Infrastructure/Services/AzureUserSyncService.cs`
