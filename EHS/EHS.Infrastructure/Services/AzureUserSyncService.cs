using EHS.Application.Interfaces;
using EHS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace EHS.Infrastructure.Services
{
    public class AzureUserSyncService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AzureUserSyncService> _logger;

        public AzureUserSyncService(UserManager<ApplicationUser> userManager, ILogger<AzureUserSyncService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApplicationUser?> SyncUserAsync(ClaimsPrincipal principal)
        {
            // Debug: Log all claims for troubleshooting
            _logger.LogDebug("Syncing user. Claims present: {Claims}",
                string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}")));

            // Extract Azure Claims
            // GetObjectId() looks for "oid" or "http://schemas.microsoft.com/identity/claims/objectidentifier"
            var oid = principal.GetObjectId();

            // Try multiple common email/username claims
            var email = principal.FindFirstValue(ClaimTypes.Email) ??
                        principal.FindFirstValue(ClaimTypes.Upn);

            var name = principal.FindFirstValue("name") ?? principal.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Azure claims missing OID or Email. OID: {OID}, Email: {Email}. Full Claims: {Claims}",
                    oid, email, string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}")));
                return null;
            }

            // 1. Try Find by AzureObjectId (Fastest, Most Reliable)
            var user = _userManager.Users.FirstOrDefault(u => u.AzureObjectId == oid);

            if (user != null)
            {
                // User Found - Update profile if changed (Sync Profile)
                await UpdateUserProfileAsync(user, name, email);
                return user;
            }

            // 2. Not found by OID. Try Find by Email (Migration Scenario: Existing Local User logging in with Azure first time)
            user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // Link Existing User to Azure OID
                _logger.LogInformation("Linking existing local user {Email} to Azure OID {OID}", email, oid);
                user.AzureObjectId = oid;
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
                return user;
            }

            // 3. Create New Shadow User (JIT - Just-In-Time Provisioning)
            return await CreateJITUserAsync(oid, email, name);
        }

        private async Task<ApplicationUser?> CreateJITUserAsync(string oid, string email, string? name)
        {
            _logger.LogInformation("Creating JIT user for {Email} (OID: {OID})", email, oid);

            var newUser = new ApplicationUser
            {
                UserName = email, // Username must be unique
                Email = email,
                AzureObjectId = oid,
                FullName = name ?? email.Split('@')[0],
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                EmailConfirmed = true // Azure already verified email
            };

            var result = await _userManager.CreateAsync(newUser);
            if (!result.Succeeded)
            {
                _logger.LogError("Failed to create JIT user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                return null;
            }

            // Assign Default Role based on business logic
            await AssignDefaultRoleAsync(newUser);

            _logger.LogInformation("JIT user created successfully: {Email} (ID: {Id})", newUser.Email, newUser.Id);

            return newUser;
        }

        private async Task AssignDefaultRoleAsync(ApplicationUser user)
        {
            // Real-world approach: All new users get "Initiator" role by default
            // Admin will assign other roles (SafetyOfficer, Implementor) through admin UI
            await _userManager.AddToRoleAsync(user, "Initiator");
            _logger.LogInformation("Assigned default Initiator role to new user: {Email}", user.Email);
        }

        private async Task UpdateUserProfileAsync(ApplicationUser user, string? name, string? email)
        {
            bool hasChanges = false;

            // Update name if changed
            if (!string.IsNullOrEmpty(name) && user.FullName != name)
            {
                _logger.LogInformation("Updating name for {Email}: {OldName} -> {NewName}",
                    user.Email, user.FullName, name);
                user.FullName = name;
                hasChanges = true;
            }

            // Update email if changed (rare, but possible)
            if (!string.IsNullOrEmpty(email) && user.Email != email)
            {
                _logger.LogInformation("Updating email for user {OldEmail} -> {NewEmail}",
                    user.Email, email);
                user.Email = email;
                user.UserName = email;
                hasChanges = true;
            }

            // Always update last login time
            user.LastLoginAt = DateTime.UtcNow;
            hasChanges = true;

            if (hasChanges)
            {
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to update user profile: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}