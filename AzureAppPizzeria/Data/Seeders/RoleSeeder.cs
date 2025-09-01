using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AzureAppPizzeria.Data.Seeders
{
    public class RoleSeeder
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var logger = serviceProvider.GetRequiredService<ILogger<RoleSeeder>>();

            string[] roleNames = { "Admin", "PremiumUser", "RegularUser" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var roleResult = await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Role {RoleName} created successfully.", roleName);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {RoleName}: {Errors}", roleName, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    logger.LogInformation("Role {RoleName} already exists.", roleName);
                }
            }
        }
    }
}
