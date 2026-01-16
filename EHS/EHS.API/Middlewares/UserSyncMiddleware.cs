using EHS.Infrastructure.Services;
using Microsoft.Identity.Web;

namespace EHS.API.Middlewares
{
    public class UserSyncMiddleware
    {
        private readonly RequestDelegate _next;

        public UserSyncMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, AzureUserSyncService syncService)
        {
            // Only run if user is authenticated (Azure Token Validated)
            if (context.User.Identity?.IsAuthenticated == true)
            {
                if (user != null)
                {
                    // Success: User is synced.
                    // CRITICAL: We must attach the LOCAL GUID to the Principal so Controllers can use it.
                    // Azure Token has OID (String), but Database needs ID (Guid).
                    
                    var appIdentity = new ClaimsIdentity();
                    appIdentity.AddClaim(new Claim("LocalUserId", user.Id.ToString()));
                    
                    // Add roles from DB too? 
                    // If we use Local Roles, we should add them here so [Authorize(Roles="Init")] works!
                    // Assuming Roles are loaded in user object? 
                    // _userManager.GetRolesAsync might be needed if they are not loaded.
                    // For performance, let's assume simple ID first. If Roles are needed, we can expand.
                    
                    context.User.AddIdentity(appIdentity);
                }
                else
                {
                     // If sync fails, what do we do? 
                     // Log and maybe failing the request is safer than crashing later on Guid.Parse
                }
            }

            await _next(context);
        }
    }
}
