using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.Services.Implementations;
using Xunit;

namespace GarageOperationsManagementSystem.Tests
{
    public class OwnerServiceTests
    {
        private static OwnerService CreateService(string dbName)
            => new OwnerService(TestDbContextFactory.Create(dbName));

        // ── GetAllAsync ──────────────────────────────────────────────────────
        [Fact]
        public async Task GetAllAsync_ReturnsAllOwners()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetAllAsync_ReturnsAllOwners));
            ctx.Owners.AddRange(new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" },
                                new Owner { FullName = "Bob",   Email = "b@x.com", PhoneNumber = "2" });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var result = await svc.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOrderedByFullName()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetAllAsync_ReturnsOrderedByFullName));
            ctx.Owners.AddRange(new Owner { FullName = "Zara", Email = "z@x.com", PhoneNumber = "1" },
                                new Owner { FullName = "Anna", Email = "a@x.com", PhoneNumber = "2" });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var result = (await svc.GetAllAsync()).ToList();

            Assert.Equal("Anna", result[0].FullName);
            Assert.Equal("Zara", result[1].FullName);
        }

        [Fact]
        public async Task GetAllAsync_IncludesCars()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetAllAsync_IncludesCars));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            ctx.Cars.Add(new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var result = (await svc.GetAllAsync()).First();

            Assert.Single(result.Cars);
        }

        // ── GetByIdAsync ─────────────────────────────────────────────────────
        [Fact]
        public async Task GetByIdAsync_ReturnsOwner_WhenExists()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetByIdAsync_ReturnsOwner_WhenExists));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var result = await svc.GetByIdAsync(owner.Id);

            Assert.NotNull(result);
            Assert.Equal("Alice", result!.FullName);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var svc = CreateService(nameof(GetByIdAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetByIdAsync(999);
            Assert.Null(result);
        }

        // ── GetByIdWithCarsAsync ──────────────────────────────────────────────
        [Fact]
        public async Task GetByIdWithCarsAsync_IncludesCars()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetByIdWithCarsAsync_IncludesCars));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            ctx.Cars.Add(new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var result = await svc.GetByIdWithCarsAsync(owner.Id);

            Assert.NotNull(result);
            Assert.Single(result!.Cars);
        }

        [Fact]
        public async Task GetByIdWithCarsAsync_ReturnsNull_WhenNotFound()
        {
            var svc = CreateService(nameof(GetByIdWithCarsAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetByIdWithCarsAsync(999);
            Assert.Null(result);
        }

        // ── CreateAsync ───────────────────────────────────────────────────────
        [Fact]
        public async Task CreateAsync_AddsOwnerToDatabase()
        {
            using var ctx = TestDbContextFactory.Create(nameof(CreateAsync_AddsOwnerToDatabase));
            var svc = new OwnerService(ctx);

            await svc.CreateAsync(new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" });

            Assert.Equal(1, ctx.Owners.Count());
        }

        // ── UpdateAsync ───────────────────────────────────────────────────────
        [Fact]
        public async Task UpdateAsync_PersistsChanges()
        {
            using var ctx = TestDbContextFactory.Create(nameof(UpdateAsync_PersistsChanges));
            var owner = new Owner { FullName = "Old Name", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            owner.FullName = "New Name";
            await svc.UpdateAsync(owner);

            Assert.Equal("New Name", ctx.Owners.Find(owner.Id)!.FullName);
        }

        // ── DeleteAsync ───────────────────────────────────────────────────────
        [Fact]
        public async Task DeleteAsync_RemovesOwner()
        {
            using var ctx = TestDbContextFactory.Create(nameof(DeleteAsync_RemovesOwner));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            await svc.DeleteAsync(owner.Id);

            Assert.Empty(ctx.Owners);
        }

        [Fact]
        public async Task DeleteAsync_DoesNotThrow_WhenNotFound()
        {
            using var ctx = TestDbContextFactory.Create(nameof(DeleteAsync_DoesNotThrow_WhenNotFound));
            var svc = new OwnerService(ctx);
            var ex = await Record.ExceptionAsync(() => svc.DeleteAsync(999));
            Assert.Null(ex);
        }

        // ── ExistsAsync ───────────────────────────────────────────────────────
        [Fact]
        public async Task ExistsAsync_ReturnsTrue_WhenOwnerExists()
        {
            using var ctx = TestDbContextFactory.Create(nameof(ExistsAsync_ReturnsTrue_WhenOwnerExists));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            Assert.True(await svc.ExistsAsync(owner.Id));
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_WhenNotFound()
        {
            var svc = CreateService(nameof(ExistsAsync_ReturnsFalse_WhenNotFound));
            Assert.False(await svc.ExistsAsync(999));
        }

        // ── HasCarsAsync ──────────────────────────────────────────────────────
        [Fact]
        public async Task HasCarsAsync_ReturnsTrue_WhenOwnerHasCars()
        {
            using var ctx = TestDbContextFactory.Create(nameof(HasCarsAsync_ReturnsTrue_WhenOwnerHasCars));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            ctx.Cars.Add(new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            Assert.True(await svc.HasCarsAsync(owner.Id));
        }

        [Fact]
        public async Task HasCarsAsync_ReturnsFalse_WhenNoCars()
        {
            using var ctx = TestDbContextFactory.Create(nameof(HasCarsAsync_ReturnsFalse_WhenNoCars));
            var owner = new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            ctx.Owners.Add(owner);
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            Assert.False(await svc.HasCarsAsync(owner.Id));
        }

        // ── GetQueryable ──────────────────────────────────────────────────────
        [Fact]
        public async Task GetQueryable_ReturnsQueryableOfOwners()
        {
            using var ctx = TestDbContextFactory.Create(nameof(GetQueryable_ReturnsQueryableOfOwners));
            ctx.Owners.Add(new Owner { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" });
            await ctx.SaveChangesAsync();
            var svc = new OwnerService(ctx);

            var query = svc.GetQueryable();

            Assert.Equal(1, query.Count());
        }
    }
}
