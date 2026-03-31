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
            string[] roles = { "Admin", "Employee", "Guest" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            //var adminName = "Admin";
            var adminEmail = "admin@garage.com";
            var adminPassword = "Admin123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                };

                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var employeeEmail = "employee@garage.com";
            var employeePassword = "Employee123!";

            if (await userManager.FindByEmailAsync(employeeEmail) == null)
            {
                var employee = new ApplicationUser()
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                };

                await userManager.CreateAsync(employee, employeePassword);
                await userManager.AddToRoleAsync(employee, "Employee");
            }

        }


    }
}