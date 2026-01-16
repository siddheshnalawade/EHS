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
            // Extract Azure Claims
            var oid = principal.GetObjectId(); // From Microsoft.Identity.Web
            var email = principal.FindFirstValue(ClaimTypes.Email) ?? principal.FindFirstValue("preferred_username"); // preferred_username is common in v2.0
            var name = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue("name");

            if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("Azure claims missing OID or Email.");
                return null;
            }

            // 1. Try Find by AzureObjectId (Fastest, Stable)
            // We need to add a method to UserManager or duplicate finding logic here if custom store implementation is limited.
            // Since we use EF, we can query users directly via UserManager if we extended it, 
            // Or just iterate (inefficient) or use normal FindByEmail.
            // The BEST way is to first try finding by 'AzureObjectId' if we could index it.
            // But 'UserManager' doesn't know about AzureObjectId property by default.
            // So we can use: _userManager.Users.FirstOrDefaultAsync(u => u.AzureObjectId == oid);
            
            // NOTE: Since I am in a service, I can't access DbSet directly unless I inject Repo or Context.
            // But UserManager exposes IQueryable "Users".
            
            var user = _userManager.Users.FirstOrDefault(u => u.AzureObjectId == oid);

            if (user != null)
            {
                // User Found - Update details if changed (Sync Profile)
                // Optionally update Name/Email if changed in Azure?
                // Let's just return for performance.
                return user;
            }

            // 2. Not found by OID. Try Find by Email (Migration Scenario: Existing Local User logging in with Azure first time)
            user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                // Link Existing User to Azure OID
                _logger.LogInformation($"Linking existing local user {email} to Azure OID {oid}.");
                user.AzureObjectId = oid;
                await _userManager.UpdateAsync(user);
                return user;
            }

            // 3. Create New Shadow User (JIT)
            _logger.LogInformation($"Creating new JIT user for {email} (OID: {oid}).");
            var newUser = new ApplicationUser
            {
                UserName = email, // Username must be unique
                Email = email,
                AzureObjectId = oid,
                FullName = name ?? email.Split('@')[0],
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                // Assign Default Role? e.g. "User"
                await _userManager.AddToRoleAsync(newUser, "User");
                return newUser;
            }

            _logger.LogError($"Failed to create JIT user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            return null;
        }
    }
}
