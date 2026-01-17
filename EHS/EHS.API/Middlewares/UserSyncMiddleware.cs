using EHS.Domain.Entities;
using EHS.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace EHS.API.Middlewares
{
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
            UserManager<ApplicationUser> userManager,
            ILogger<UserSyncMiddleware> logger)
        {
            // Only run if user is authenticated (Azure Token Validated)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                try
                {
                    // Sync user from Azure AD to local database
                    var user = await syncService.SyncUserAsync(context.User);
                    
                    if (user == null)
                    {
                        logger.LogWarning("User synchronization failed for authenticated user");
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "User synchronization failed",
                            message = "Unable to create or sync user account. Please contact administrator."
                        });
                        return;
                    }
                    
                    // Check if user account is active
                    if (!user.IsActive)
                    {
                        logger.LogWarning("Inactive user attempted to access: {Email}", user.Email);
                        context.Response.StatusCode = 403;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            error = "Account disabled",
                            message = "Your account has been disabled. Please contact administrator."
                        });
                        return;
                    }
                    
                    // Success: User is synced.
                    // CRITICAL: We must attach the LOCAL GUID to the Principal so Controllers can use it.
                    // Azure Token has OID (String), but Database needs ID (Guid).
                    
                    var appIdentity = new ClaimsIdentity();
                    appIdentity.AddClaim(new Claim("LocalUserId", user.Id.ToString()));
                    
                    // Add local roles as claims so [Authorize(Roles="Initiator")] works
                    var roles = await userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        appIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                    
                    // Add additional user info claims
                    appIdentity.AddClaim(new Claim("FullName", user.FullName ?? string.Empty));
                    appIdentity.AddClaim(new Claim("Email", user.Email ?? string.Empty));
                    
                    context.User.AddIdentity(appIdentity);
                    
                    logger.LogDebug("User synced successfully: {Email} (LocalId: {Id})", user.Email, user.Id);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error in UserSyncMiddleware");
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = "Internal server error",
                        message = "An error occurred during authentication. Please try again."
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}
