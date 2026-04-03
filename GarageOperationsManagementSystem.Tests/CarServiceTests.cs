using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.Services.Implementations;
using Xunit;

namespace GarageOperationsManagementSystem.Tests
{
    public class CarServiceTests
    {
        private static (CarService svc, Data.ApplicationDbContext ctx) Create(string dbName)
        {
            var ctx = TestDbContextFactory.Create(dbName);
            return (new CarService(ctx), ctx);
        }

        private static Owner SeedOwner(Data.ApplicationDbContext ctx)
        {
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            ctx.SaveChanges();
            return owner;
        }

        // ── GetAllCarsAsync ───────────────────────────────────────────────────
        [Fact]
        public async Task GetAllCarsAsync_ReturnsAllCars()
        {
            var (svc, ctx) = Create(nameof(GetAllCarsAsync_ReturnsAllCars));
            var owner = SeedOwner(ctx);
            ctx.Cars.AddRange(
                new Car { Brand = "Toyota", Model = "Yaris",  OwnerId = owner.Id },
                new Car { Brand = "Honda",  Model = "Civic",  OwnerId = owner.Id });
            await ctx.SaveChangesAsync();

            var result = await svc.GetAllCarsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllCarsAsync_IncludesOwner()
        {
            var (svc, ctx) = Create(nameof(GetAllCarsAsync_IncludesOwner));
            var owner = SeedOwner(ctx);
            ctx.Cars.Add(new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id });
            await ctx.SaveChangesAsync();

            var result = (await svc.GetAllCarsAsync()).First();

            Assert.NotNull(result.Owner);
            Assert.Equal("Alice", result.Owner.FullName);
        }

        [Fact]
        public async Task GetAllCarsAsync_ReturnsEmpty_WhenNoCars()
        {
            var (svc, _) = Create(nameof(GetAllCarsAsync_ReturnsEmpty_WhenNoCars));
            var result = await svc.GetAllCarsAsync();
            Assert.Empty(result);
        }

        // ── GetCarByIdAsync ───────────────────────────────────────────────────
        [Fact]
        public async Task GetCarByIdAsync_ReturnsCar_WhenExists()
        {
            var (svc, ctx) = Create(nameof(GetCarByIdAsync_ReturnsCar_WhenExists));
            var owner = SeedOwner(ctx);
            var car = new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id };
            ctx.Cars.Add(car);
            await ctx.SaveChangesAsync();

            var result = await svc.GetCarByIdAsync(car.Id);

            Assert.NotNull(result);
            Assert.Equal("Toyota", result!.Brand);
        }

        [Fact]
        public async Task GetCarByIdAsync_ReturnsNull_WhenNotFound()
        {
            var (svc, _) = Create(nameof(GetCarByIdAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetCarByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCarByIdAsync_IncludesOwner()
        {
            var (svc, ctx) = Create(nameof(GetCarByIdAsync_IncludesOwner));
            var owner = SeedOwner(ctx);
            var car = new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id };
            ctx.Cars.Add(car);
            await ctx.SaveChangesAsync();

            var result = await svc.GetCarByIdAsync(car.Id);

            Assert.NotNull(result!.Owner);
        }

        // ── CreateCarAsync ────────────────────────────────────────────────────
        [Fact]
        public async Task CreateCarAsync_AddsCarToDatabase()
        {
            var (svc, ctx) = Create(nameof(CreateCarAsync_AddsCarToDatabase));
            var owner = SeedOwner(ctx);

            await svc.CreateCarAsync(new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id });

            Assert.Equal(1, ctx.Cars.Count());
        }

        // ── UpdateCarAsync ────────────────────────────────────────────────────
        [Fact]
        public async Task UpdateCarAsync_PersistsChanges()
        {
            var (svc, ctx) = Create(nameof(UpdateCarAsync_PersistsChanges));
            var owner = SeedOwner(ctx);
            var car = new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id };
            ctx.Cars.Add(car);
            await ctx.SaveChangesAsync();

            car.Model = "Corolla";
            await svc.UpdateCarAsync(car);

            Assert.Equal("Corolla", ctx.Cars.Find(car.Id)!.Model);
        }

        // ── DeleteCarAsync ────────────────────────────────────────────────────
        [Fact]
        public async Task DeleteCarAsync_RemovesCar()
        {
            var (svc, ctx) = Create(nameof(DeleteCarAsync_RemovesCar));
            var owner = SeedOwner(ctx);
            var car = new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id };
            ctx.Cars.Add(car);
            await ctx.SaveChangesAsync();

            await svc.DeleteCarAsync(car.Id);

            Assert.Empty(ctx.Cars);
        }

        [Fact]
        public async Task DeleteCarAsync_DoesNotThrow_WhenNotFound()
        {
            var (svc, _) = Create(nameof(DeleteCarAsync_DoesNotThrow_WhenNotFound));
            var ex = await Record.ExceptionAsync(() => svc.DeleteCarAsync(999));
            Assert.Null(ex);
        }
    }
}
