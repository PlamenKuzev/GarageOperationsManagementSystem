using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.Services.Implementations;
using Xunit;

namespace GarageOperationsManagementSystem.Tests
{
    public class EmployeeServiceTests
    {
        private static (EmployeeService svc, Data.ApplicationDbContext ctx) Create(string dbName)
        {
            var ctx = TestDbContextFactory.Create(dbName);
            return (new EmployeeService(ctx), ctx);
        }

        private static Garage SeedGarage(Data.ApplicationDbContext ctx)
        {
            var garage = new Garage
            {
                City = "Sofia", Address = "1 Main St", Capacity = 5, WorkSchedule = "9-18"
            };
            ctx.Garages.Add(garage);
            ctx.SaveChanges();
            return garage;
        }

        private static Employee MakeEmployee(int garageId, string name = "John", string? userId = null)
            => new Employee
            {
                Name = name,
                Position = "Mechanic",
                Salary = 2000m,
                WorkingSince = DateTime.Today.AddYears(-1),
                GarageId = garageId,
                ApplicationUserId = userId
            };

        [Fact]
        public async Task GetAllAsync_ReturnsAllEmployees()
        {
            var (svc, ctx) = Create(nameof(GetAllAsync_ReturnsAllEmployees));
            var garage = SeedGarage(ctx);
            ctx.Employees.AddRange(MakeEmployee(garage.Id, "Alice"), MakeEmployee(garage.Id, "Bob"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOrderedByName()
        {
            var (svc, ctx) = Create(nameof(GetAllAsync_ReturnsOrderedByName));
            var garage = SeedGarage(ctx);
            ctx.Employees.AddRange(MakeEmployee(garage.Id, "Zara"), MakeEmployee(garage.Id, "Anna"));
            await ctx.SaveChangesAsync();

            var result = (await svc.GetAllAsync()).ToList();

            Assert.Equal("Anna", result[0].Name);
            Assert.Equal("Zara", result[1].Name);
        }

        [Fact]
        public async Task GetAllAsync_IncludesGarage()
        {
            var (svc, ctx) = Create(nameof(GetAllAsync_IncludesGarage));
            var garage = SeedGarage(ctx);
            ctx.Employees.Add(MakeEmployee(garage.Id));
            await ctx.SaveChangesAsync();

            var result = (await svc.GetAllAsync()).First();

            Assert.NotNull(result.Garage);
            Assert.Equal("Sofia", result.Garage.City);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEmployee_WhenExists()
        {
            var (svc, ctx) = Create(nameof(GetByIdAsync_ReturnsEmployee_WhenExists));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id);
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            var result = await svc.GetByIdAsync(emp.Id);

            Assert.NotNull(result);
            Assert.Equal("John", result!.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var (svc, _) = Create(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsEmployee_WhenUserIdMatches()
        {
            var (svc, ctx) = Create(nameof(GetByUserIdAsync_ReturnsEmployee_WhenUserIdMatches));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id, userId: "user-abc");
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            var result = await svc.GetByUserIdAsync("user-abc");

            Assert.NotNull(result);
            Assert.Equal("user-abc", result!.ApplicationUserId);
        }

        [Fact]
        public async Task GetByUserIdAsync_ReturnsNull_WhenNoMatch()
        {
            var (svc, _) = Create(nameof(GetByUserIdAsync_ReturnsNull_WhenNoMatch));
            var result = await svc.GetByUserIdAsync("nonexistent");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_IncludesGarage()
        {
            var (svc, ctx) = Create(nameof(GetByUserIdAsync_IncludesGarage));
            var garage = SeedGarage(ctx);
            ctx.Employees.Add(MakeEmployee(garage.Id, userId: "user-xyz"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetByUserIdAsync("user-xyz");

            Assert.NotNull(result!.Garage);
        }

        [Fact]
        public async Task CreateAsync_AddsEmployee()
        {
            var (svc, ctx) = Create(nameof(CreateAsync_AddsEmployee));
            var garage = SeedGarage(ctx);

            await svc.CreateAsync(MakeEmployee(garage.Id));

            Assert.Equal(1, ctx.Employees.Count());
        }

        [Fact]
        public async Task UpdateAsync_PersistsChanges()
        {
            var (svc, ctx) = Create(nameof(UpdateAsync_PersistsChanges));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id);
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            emp.Position = "Senior Mechanic";
            await svc.UpdateAsync(emp);

            Assert.Equal("Senior Mechanic", ctx.Employees.Find(emp.Id)!.Position);
        }

        [Fact]
        public async Task UpdateAsync_CanSetIsTrusted()
        {
            var (svc, ctx) = Create(nameof(UpdateAsync_CanSetIsTrusted));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id);
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            emp.IsTrusted = true;
            await svc.UpdateAsync(emp);

            Assert.True(ctx.Employees.Find(emp.Id)!.IsTrusted);
        }

        [Fact]
        public async Task DeleteAsync_RemovesEmployee()
        {
            var (svc, ctx) = Create(nameof(DeleteAsync_RemovesEmployee));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id);
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            await svc.DeleteAsync(emp.Id);

            Assert.Empty(ctx.Employees);
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow_WhenNotFound()
        {
            var (svc, _) = Create(nameof(DeleteAsync_DoesNotThrow_WhenNotFound));
            var ex = await Record.ExceptionAsync(() => svc.DeleteAsync(999));
            Assert.Null(ex);
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenExists()
        {
            var (svc, ctx) = Create(nameof(ExistsAsync_ReturnsTrue_WhenExists));
            var garage = SeedGarage(ctx);
            var emp = MakeEmployee(garage.Id);
            ctx.Employees.Add(emp);
            await ctx.SaveChangesAsync();

            Assert.True(await svc.ExistsAsync(emp.Id));
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenNotFound()
        {
            var (svc, _) = Create(nameof(ExistsAsync_ReturnsFalse_WhenNotFound));
            Assert.False(await svc.ExistsAsync(999));
        }
    }
}