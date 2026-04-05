using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext dbContext)
        {
            string[] roles = { "Admin", "Employee", "Guest" };

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
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(admin, adminPassword);
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            var employeeEmail = "employee@garage.com";
            var employeePassword = "Employee123!";

            ApplicationUser? empUser = await userManager.FindByEmailAsync(employeeEmail);
            if (empUser == null)
            {
                empUser = new ApplicationUser
                {
                    UserName = employeeEmail,
                    Email = employeeEmail,
                    FullName = "Demo Employee",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(empUser, employeePassword);
                await userManager.AddToRoleAsync(empUser, "Employee");
            }

            // Link the demo employee user to an Employee record if not already linked
            var hasRecord = await dbContext.Employees
                .AnyAsync(e => e.ApplicationUserId == empUser.Id);

            if (!hasRecord)
            {
                var firstGarage = await dbContext.Garages.FirstOrDefaultAsync();
                if (firstGarage != null)
                {
                    dbContext.Employees.Add(new Employee
                    {
                        Name = "Demo Employee",
                        Position = "Mechanic",
                        Salary = 2000m,
                        WorkingSince = DateTime.Now.AddYears(-2),
                        GarageId = firstGarage.Id,
                        ApplicationUserId = empUser.Id,
                        IsTrusted = false
                    });
                    await dbContext.SaveChangesAsync();
                }
            }

            var trustedEmail = "trustedEmployee@garage.com";
            var trustedPassword = "TrustedEmployee123!";

            ApplicationUser? trustedUser = await userManager.FindByEmailAsync(trustedEmail);
            if (trustedUser == null)
            {
                trustedUser = new ApplicationUser
                {
                    UserName = trustedEmail,
                    Email = trustedEmail,
                    FullName = "Trusted Employee",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(trustedUser, trustedPassword);
                await userManager.AddToRoleAsync(trustedUser, "Employee");
            }

            var hasTrustedRecord = await dbContext.Employees
                .AnyAsync(e => e.ApplicationUserId == trustedUser.Id);

            if (!hasTrustedRecord)
            {
                var firstGarage = await dbContext.Garages.FirstOrDefaultAsync();
                if (firstGarage != null)
                {
                    dbContext.Employees.Add(new Employee
                    {
                        Name = "Trusted Employee",
                        Position = "Senior Mechanic",
                        Salary = 3000m,
                        WorkingSince = DateTime.Now.AddYears(-5),
                        GarageId = firstGarage.Id,
                        ApplicationUserId = trustedUser.Id,
                        IsTrusted = true
                    });
                    await dbContext.SaveChangesAsync();
                }
            }

            // Seed a demo guest user linked to the demo owner
            var guestEmail = "owner@garage.com";
            var guestPassword = "Owner123!";

            ApplicationUser? guestUser = await userManager.FindByEmailAsync(guestEmail);
            if (guestUser == null)
            {
                guestUser = new ApplicationUser
                {
                    UserName = guestEmail,
                    Email = guestEmail,
                    FullName = "Demo Owner",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(guestUser, guestPassword);
                await userManager.AddToRoleAsync(guestUser, "Guest");
            }

            // Link the guest user to the Demo Owner record if not already linked
            var demoOwner = await dbContext.Owners
                .FirstOrDefaultAsync(o => o.FullName == "Demo Owner" && o.ApplicationUserId == null);

            if (demoOwner != null && guestUser != null)
            {
                demoOwner.ApplicationUserId = guestUser.Id;
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
