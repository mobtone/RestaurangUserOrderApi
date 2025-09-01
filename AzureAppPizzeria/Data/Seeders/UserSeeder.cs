using AzureAppPizzeria.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace AzureAppPizzeria.Data.Seeders
{
    public class UserSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>(); // Se till att ApplicationRole är din rollklass
            var logger = serviceProvider.GetRequiredService<ILogger<RoleSeeder>>(); // Eller en ILogger<UserSeeder>

            string adminEmail = "admin@tomasos.com";
            string adminUsername = "admin";
            string adminPassword = "AdminPassword123!"; // Ändra detta till ett starkt lösenord och hantera säkert!
                                                        // För produktion, överväg att sätta detta via konfiguration/Key Vault
                                                        // och bara skapa om det inte finns.

            // Se till att Admin-rollen finns
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
                logger.LogInformation("Role 'Admin' created during admin user seeding.");
            }

            // Kolla om admin-användaren redan finns
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                ApplicationUser adminUser = new ApplicationUser
                {
                    UserName = adminUsername,
                    Email = adminEmail,
                    EmailConfirmed = true, // Admin-konto kan vara bekräftat direkt
                                           // DisplayName = "System Administrator" // Om du har DisplayName
                };

                IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    logger.LogInformation("Admin user {AdminEmail} created successfully.", adminEmail);
                    // Lägg till admin-användaren i "Admin"-rollen
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    logger.LogInformation("Admin user {AdminEmail} added to 'Admin' role.", adminEmail);
                }
                else
                {
                    logger.LogError("Error creating admin user {AdminEmail}. Errors: {Errors}", adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                logger.LogInformation("Admin user {AdminEmail} already exists.", adminEmail);
            }
        }
    }
}
