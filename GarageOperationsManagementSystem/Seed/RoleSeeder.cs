using Azure.Identity;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace GarageOperationsManagementSystem.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roles = { "Admin", "Employee", "Customer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@garage.com";
            var adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser()
                {
                    UserName = adminEmail,
                    Email = adminPassword,
                    PasswordHash = adminPassword
                };

                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }


    }
}