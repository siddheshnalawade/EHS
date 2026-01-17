# Azure Entra ID + Local Database User Management Strategy

## 🎯 Overview

For enterprise applications (EHS, CMMS, QC) in an aluminum casting company, you need a **hybrid authentication model**:

- **Azure Entra ID** - Single source of truth for authentication (SSO across all apps)
- **Local Database** - Application-specific user profiles, roles, and permissions

---

## 🏗️ Architecture: Just-In-Time (JIT) User Provisioning

### How It Works

```
┌─────────────────────────────────────────────────────────────┐
│                    USER LOGIN FLOW                          │
└─────────────────────────────────────────────────────────────┘

1. User logs in → Azure Entra ID
   ↓
2. Azure validates credentials → Issues JWT token
   ↓
3. User accesses EHS/CMMS/QC app → Sends token
   ↓
4. App validates token → Extracts Azure claims (OID, email, name)
   ↓
5. UserSyncMiddleware checks local database:
   ├─ User exists (by AzureObjectId) → Use existing
   ├─ User exists (by Email, no OID) → Link to Azure OID
   └─ User doesn't exist → Create new user (JIT Provisioning)
   ↓
6. Add LocalUserId claim → Continue to controller
```

---

## 📊 Real-World Approaches

### Option 1: **Just-In-Time (JIT) Provisioning** ⭐ RECOMMENDED

**What:** Automatically create users in local DB on first login.

**Pros:**
- ✅ Zero manual work
- ✅ Users auto-created when they first access the app
- ✅ Always in sync with Azure Entra ID
- ✅ Works across multiple apps (EHS, CMMS, QC)

**Cons:**
- ⚠️ New users get default role (need admin to assign proper roles)
- ⚠️ No pre-provisioning (user must login first)

**Best For:** Your scenario (multiple apps, same user base)

---

### Option 2: **Pre-Provisioning via Admin Portal**

**What:** Admin manually creates users in local DB before they login.

**Pros:**
- ✅ Full control over user creation
- ✅ Roles assigned before first login
- ✅ Can provision users who haven't logged in yet

**Cons:**
- ❌ Manual work required
- ❌ Can get out of sync with Azure Entra ID
- ❌ Duplicate effort

**Best For:** Small teams, high security requirements

---

### Option 3: **Automated Sync via Microsoft Graph API**

**What:** Background job syncs users from Azure Entra ID to local DB.

**Pros:**
- ✅ Fully automated
- ✅ Users pre-provisioned
- ✅ Can sync groups/departments from Azure AD

**Cons:**
- ❌ Complex to implement
- ❌ Requires Microsoft Graph API permissions
- ❌ Scheduled sync (not real-time)

**Best For:** Large organizations (1000+ users)

---

### Option 4: **Hybrid: JIT + Admin Override**

**What:** JIT provisioning + admin can manually create/edit users.

**Pros:**
- ✅ Best of both worlds
- ✅ Automatic for most users
- ✅ Manual control when needed

**Cons:**
- ⚠️ Slightly more complex

**Best For:** Your scenario ⭐ **RECOMMENDED**

---

## 🔧 Implementation: Hybrid JIT + Admin Override

### 1. Database Schema

Your current `ApplicationUser` is good, but let's enhance it:

```csharp
public class ApplicationUser : IdentityUser<Guid>
{
    // Azure Entra ID Link
    public string? AzureObjectId { get; set; } // OID from Azure
    
    // Profile
    public string FullName { get; set; } = string.Empty;
    public string? EmployeeId { get; set; } // Company employee ID
    public string? Department { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    
    // Status
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    
    // Provisioning Info
    public UserProvisioningSource ProvisioningSource { get; set; } // JIT or Manual
    public DateTime? LastSyncedAt { get; set; }
    
    // Multi-App Access
    public bool HasEHSAccess { get; set; } = false;
    public bool HasCMMSAccess { get; set; } = false;
    public bool HasQCAccess { get; set; } = false;
}

public enum UserProvisioningSource
{
    Manual = 0,      // Created by admin
    AzureJIT = 1,    // Auto-created on first login
    GraphSync = 2    // Synced via Microsoft Graph API
}
```

### 2. Enhanced AzureUserSyncService

```csharp
public class AzureUserSyncService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ILogger<AzureUserSyncService> _logger;
    
    public async Task<ApplicationUser?> SyncUserAsync(ClaimsPrincipal principal)
    {
        // Extract Azure claims
        var oid = principal.GetObjectId();
        var email = principal.FindFirstValue(ClaimTypes.Email) 
                    ?? principal.FindFirstValue("preferred_username");
        var name = principal.FindFirstValue(ClaimTypes.Name) 
                   ?? principal.FindFirstValue("name");
        var jobTitle = principal.FindFirstValue("jobTitle");
        var department = principal.FindFirstValue("department");
        
        if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(email))
        {
            _logger.LogWarning("Azure claims missing OID or Email");
            return null;
        }
        
        // 1. Find by AzureObjectId (primary key)
        var user = _userManager.Users
            .FirstOrDefault(u => u.AzureObjectId == oid);
        
        if (user != null)
        {
            // Update profile if changed
            await UpdateUserProfileAsync(user, name, email, jobTitle, department);
            return user;
        }
        
        // 2. Find by Email (migration scenario)
        user = await _userManager.FindByEmailAsync(email);
        
        if (user != null)
        {
            // Link existing user to Azure
            _logger.LogInformation("Linking user {Email} to Azure OID {OID}", email, oid);
            user.AzureObjectId = oid;
            user.LastSyncedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            return user;
        }
        
        // 3. Create new user (JIT Provisioning)
        return await CreateJITUserAsync(oid, email, name, jobTitle, department);
    }
    
    private async Task<ApplicationUser?> CreateJITUserAsync(
        string oid, string email, string? name, string? jobTitle, string? department)
    {
        _logger.LogInformation("Creating JIT user for {Email}", email);
        
        var newUser = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true, // Azure already verified
            AzureObjectId = oid,
            FullName = name ?? email.Split('@')[0],
            JobTitle = jobTitle,
            Department = department,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LastSyncedAt = DateTime.UtcNow,
            ProvisioningSource = UserProvisioningSource.AzureJIT,
            
            // Default: No app access (admin must grant)
            HasEHSAccess = false,
            HasCMMSAccess = false,
            HasQCAccess = false
        };
        
        var result = await _userManager.CreateAsync(newUser);
        
        if (!result.Succeeded)
        {
            _logger.LogError("Failed to create JIT user: {Errors}", 
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return null;
        }
        
        // Assign default role
        await AssignDefaultRoleAsync(newUser);
        
        // Send notification to admin
        await NotifyAdminOfNewUserAsync(newUser);
        
        return newUser;
    }
    
    private async Task AssignDefaultRoleAsync(ApplicationUser user)
    {
        // Option 1: Assign based on email domain
        if (user.Email?.EndsWith("@admin.company.com") == true)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
        }
        else
        {
            // Default role for new users
            await _userManager.AddToRoleAsync(user, "User");
        }
        
        // Option 2: Assign based on Azure AD group membership
        // (requires Microsoft Graph API integration)
    }
    
    private async Task UpdateUserProfileAsync(
        ApplicationUser user, string? name, string? email, 
        string? jobTitle, string? department)
    {
        bool hasChanges = false;
        
        if (!string.IsNullOrEmpty(name) && user.FullName != name)
        {
            user.FullName = name;
            hasChanges = true;
        }
        
        if (!string.IsNullOrEmpty(email) && user.Email != email)
        {
            user.Email = email;
            user.UserName = email;
            hasChanges = true;
        }
        
        if (!string.IsNullOrEmpty(jobTitle) && user.JobTitle != jobTitle)
        {
            user.JobTitle = jobTitle;
            hasChanges = true;
        }
        
        if (!string.IsNullOrEmpty(department) && user.Department != department)
        {
            user.Department = department;
            hasChanges = true;
        }
        
        if (hasChanges)
        {
            user.LastSyncedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
            _logger.LogInformation("Updated profile for user {Email}", user.Email);
        }
        
        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
    
    private async Task NotifyAdminOfNewUserAsync(ApplicationUser user)
    {
        // Send email to admin about new user needing role assignment
        // Implementation depends on your email service
    }
}
```

### 3. Fixed UserSyncMiddleware

```csharp
public class UserSyncMiddleware
{
    private readonly RequestDelegate _next;
    
    public UserSyncMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(
        HttpContext context, 
        AzureUserSyncService syncService,
        UserManager<ApplicationUser> userManager)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                // Sync user from Azure to local DB
                var user = await syncService.SyncUserAsync(context.User);
                
                if (user == null)
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "User synchronization failed",
                        message = "Unable to create or sync user account"
                    });
                    return;
                }
                
                // Check if user is active
                if (!user.IsActive)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Account disabled",
                        message = "Your account has been disabled. Contact administrator."
                    });
                    return;
                }
                
                // Add local user ID claim
                var appIdentity = new ClaimsIdentity();
                appIdentity.AddClaim(new Claim("LocalUserId", user.Id.ToString()));
                
                // Add local roles as claims
                var roles = await userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    appIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }
                
                // Add app access claims
                if (user.HasEHSAccess)
                    appIdentity.AddClaim(new Claim("AppAccess", "EHS"));
                if (user.HasCMMSAccess)
                    appIdentity.AddClaim(new Claim("AppAccess", "CMMS"));
                if (user.HasQCAccess)
                    appIdentity.AddClaim(new Claim("AppAccess", "QC"));
                
                context.User.AddIdentity(appIdentity);
            }
            catch (Exception ex)
            {
                // Log error but don't block request
                var logger = context.RequestServices
                    .GetRequiredService<ILogger<UserSyncMiddleware>>();
                logger.LogError(ex, "Error in UserSyncMiddleware");
                
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal server error",
                    message = "An error occurred during authentication"
                });
                return;
            }
        }
        
        await _next(context);
    }
}
```

---

## 🔐 Multi-Application Access Control

### Shared User Database Approach

For EHS, CMMS, and QC applications:

```
┌─────────────────────────────────────────────────────────────┐
│              SHARED USER DATABASE                           │
├─────────────────────────────────────────────────────────────┤
│  ApplicationUser                                            │
│  ├─ AzureObjectId (link to Azure Entra ID)                 │
│  ├─ Email, FullName, Department                            │
│  ├─ HasEHSAccess                                            │
│  ├─ HasCMMSAccess                                           │
│  ├─ HasQCAccess                                             │
│  └─ Roles (per application)                                 │
└─────────────────────────────────────────────────────────────┘
         ↓                    ↓                    ↓
    ┌─────────┐         ┌─────────┐         ┌─────────┐
    │   EHS   │         │  CMMS   │         │   QC    │
    │   App   │         │   App   │         │   App   │
    └─────────┘         └─────────┘         └─────────┘
```

### Implementation

```csharp
// In each app's middleware
public async Task InvokeAsync(HttpContext context, ...)
{
    var user = await syncService.SyncUserAsync(context.User);
    
    // Check app-specific access
    var appName = context.Request.Headers["X-App-Name"].ToString();
    
    bool hasAccess = appName switch
    {
        "EHS" => user.HasEHSAccess,
        "CMMS" => user.HasCMMSAccess,
        "QC" => user.HasQCAccess,
        _ => false
    };
    
    if (!hasAccess)
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Access denied",
            message = $"You don't have access to {appName} application"
        });
        return;
    }
    
    // Continue...
}
```

---

## 👨‍💼 Admin User Management

### Create Admin API Endpoints

```csharp
[ApiController]
[Route("api/v1/admin/users")]
[Authorize(Roles = "Admin")]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    // List all users
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int size = 20)
    {
        var users = await _userManager.Users
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();
            
        return Ok(users);
    }
    
    // Grant app access
    [HttpPost("{userId}/grant-access")]
    public async Task<IActionResult> GrantAppAccess(
        Guid userId, 
        [FromBody] GrantAccessRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();
        
        if (request.AppName == "EHS")
            user.HasEHSAccess = true;
        else if (request.AppName == "CMMS")
            user.HasCMMSAccess = true;
        else if (request.AppName == "QC")
            user.HasQCAccess = true;
            
        await _userManager.UpdateAsync(user);
        return Ok();
    }
    
    // Assign role
    [HttpPost("{userId}/assign-role")]
    public async Task<IActionResult> AssignRole(
        Guid userId, 
        [FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return NotFound();
        
        await _userManager.AddToRoleAsync(user, request.RoleName);
        return Ok();
    }
    
    // Manually create user (pre-provisioning)
    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            EmployeeId = request.EmployeeId,
            Department = request.Department,
            JobTitle = request.JobTitle,
            IsActive = true,
            ProvisioningSource = UserProvisioningSource.Manual,
            HasEHSAccess = request.HasEHSAccess,
            HasCMMSAccess = request.HasCMMSAccess,
            HasQCAccess = request.HasQCAccess
        };
        
        var result = await _userManager.CreateAsync(user);
        
        if (!result.Succeeded)
            return BadRequest(result.Errors);
            
        // Assign roles
        foreach (var role in request.Roles)
        {
            await _userManager.AddToRoleAsync(user, role);
        }
        
        return Ok(user);
    }
}
```

---

## 🔄 User Lifecycle Management

### Scenario 1: New Employee Joins

```
1. HR creates user in Azure Entra ID
   ↓
2. User logs into EHS app for first time
   ↓
3. JIT provisioning creates local user (HasEHSAccess = false)
   ↓
4. Admin receives notification
   ↓
5. Admin grants EHS access and assigns role (Initiator)
   ↓
6. User can now use EHS app
```

### Scenario 2: Employee Changes Department

```
1. HR updates department in Azure Entra ID
   ↓
2. User logs into app
   ↓
3. UserSyncMiddleware updates local profile
   ↓
4. Department field synced automatically
```

### Scenario 3: Employee Leaves Company

```
1. HR disables user in Azure Entra ID
   ↓
2. User can no longer login (Azure blocks)
   ↓
3. Admin marks user as inactive in local DB (optional)
   ↓
4. Historical data preserved (audit trail)
```

---

## 📊 Recommended Approach for Your Scenario

### ✅ Use: Hybrid JIT + Admin Override

1. **JIT Provisioning** - Auto-create users on first login
2. **Default State** - No app access, "User" role
3. **Admin Portal** - Grant app access (EHS/CMMS/QC) and assign roles
4. **Profile Sync** - Auto-update name, email, department from Azure
5. **Shared Database** - One user table for all three apps

### Benefits

- ✅ Zero manual work for user creation
- ✅ Admin controls app access and roles
- ✅ Always in sync with Azure Entra ID
- ✅ Works across EHS, CMMS, QC
- ✅ Audit trail preserved
- ✅ Scalable to 1000+ users

---

## 🚀 Implementation Checklist

- [ ] Update `ApplicationUser` entity with new fields
- [ ] Fix `UserSyncMiddleware` (add missing `user` variable)
- [ ] Enhance `AzureUserSyncService` with profile sync
- [ ] Create admin user management API
- [ ] Add app access control (HasEHSAccess, etc.)
- [ ] Create admin UI for user management
- [ ] Set up email notifications for new users
- [ ] Test JIT provisioning flow
- [ ] Test profile sync on login
- [ ] Document user management process

---

**Generated:** 2026-01-17
**Purpose:** User management strategy for multi-application environment
