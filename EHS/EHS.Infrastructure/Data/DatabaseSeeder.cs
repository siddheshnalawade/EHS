using EHS.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EHS.Infrastructure.Data
{
    /// <summary>
    /// Seeds initial data for the EHS application
    /// - Creates default roles
    /// - Creates initial admin user
    /// </summary>
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger logger)
        {
            try
            {
                // 1. Seed Roles
                await SeedRolesAsync(roleManager, logger);

                // 2. Seed Admin User
                await SeedAdminUserAsync(userManager, logger);

                logger.LogInformation("Database seeding completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding database");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            string[] roles = { "Admin", "SafetyOfficer", "Implementor", "Initiator" };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new ApplicationRole { Name = roleName };
                    var result = await roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Created role: {RoleName}", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {RoleName}: {Errors}",
                            roleName,
                            string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogDebug("Role {RoleName} already exists", roleName);
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, ILogger logger)
        {
            // Check if admin user already exists
            var adminEmail = "admin@ehs.com"; // Change this to your desired admin email
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin != null)
            {
                logger.LogDebug("Admin user already exists: {Email}", adminEmail);
                return;
            }

            // Create admin user
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "System Administrator",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow
            };

            // Note: For Azure AD authentication, we don't set a password
            // The admin will login through Azure AD first, then their account will be linked
            var result = await userManager.CreateAsync(adminUser);

            if (result.Succeeded)
            {
                // Assign Admin role
                await userManager.AddToRoleAsync(adminUser, "Admin");
                logger.LogInformation("Created admin user: {Email}", adminEmail);
                logger.LogWarning("IMPORTANT: Admin user created. On first login via Azure AD, this account will be linked to Azure user with email: {Email}", adminEmail);
            }
            else
            {
                logger.LogError("Failed to create admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
