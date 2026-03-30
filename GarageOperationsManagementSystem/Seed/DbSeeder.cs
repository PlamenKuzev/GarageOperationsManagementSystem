using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedDemoRepairOrdersAsync(ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();

            var demoCodes = new[] { "DEMO0001", "DEMO0002", "DEMO0003" };
            var existing = await context.RepairOrders
                .Where(r => demoCodes.Contains(r.IssueCode))
                .ToListAsync();

            if (existing.Count == demoCodes.Length)
            {
                return;
            }

            var garage = await context.Garages.FirstOrDefaultAsync();
            if (garage is null)
            {
                garage = new Garage
                {
                    City = "Sofia",
                    Address = "Demo Blvd 1",
                    Capacity = 10,
                    WorkSchedule = "Mon-Fri 08:00-17:00",
                    Latitude = 42.697708,
                    Longitude = 23.321868
                };

                context.Garages.Add(garage);
                await context.SaveChangesAsync();
            }

            var owner = await context.Owners.FirstOrDefaultAsync(o => o.Email == "demo.owner@garage.com");
            if (owner is null)
            {
                owner = new Owner
                {
                    FullName = "Demo Owner",
                    Email = "demo.owner@garage.com",
                    PhoneNumber = "+359000000000"
                };
                context.Owners.Add(owner);
                await context.SaveChangesAsync();
            }

            var car = await context.Cars.FirstOrDefaultAsync(c => c.OwnerId == owner.Id && c.Brand == "Toyota" && c.Model == "Corolla");
            if (car is null)
            {
                car = new Car
                {
                    Brand = "Toyota",
                    Model = "Corolla",
                    Year = 2016,
                    Mileage = 120_000,
                    OwnerId = owner.Id
                };
                context.Cars.Add(car);
                await context.SaveChangesAsync();
            }

            var ordersToAdd = new List<RepairOrder>();

            if (!existing.Any(r => r.IssueCode == "DEMO0001"))
            {
                ordersToAdd.Add(new RepairOrder
                {
                    IssueCode = "DEMO0001",
                    IssueDescription = "Oil change + inspection",
                    ArrivalDate = DateTime.Now.AddDays(-2),
                    IsCompleted = false,
                    CarId = car.Id,
                    GarageId = garage.Id
                });
            }

            if (!existing.Any(r => r.IssueCode == "DEMO0002"))
            {
                ordersToAdd.Add(new RepairOrder
                {
                    IssueCode = "DEMO0002",
                    IssueDescription = "Brake pads replacement",
                    ArrivalDate = DateTime.Now.AddDays(-7),
                    IsCompleted = true,
                    CompletionDate = DateTime.Now.AddDays(-5),
                    RepairPrice = 180m,
                    CarId = car.Id,
                    GarageId = garage.Id
                });
            }

            if (!existing.Any(r => r.IssueCode == "DEMO0003"))
            {
                ordersToAdd.Add(new RepairOrder
                {
                    IssueCode = "DEMO0003",
                    IssueDescription = "Engine diagnostics",
                    ArrivalDate = DateTime.Now.AddDays(-1),
                    IsCompleted = false,
                    CarId = car.Id,
                    GarageId = garage.Id
                });
            }

            if (ordersToAdd.Count > 0)
            {
                context.RepairOrders.AddRange(ordersToAdd);
                await context.SaveChangesAsync();
            }
        }
    }
}
