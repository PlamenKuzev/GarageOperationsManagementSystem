using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.Services.Implementations;
using Xunit;

namespace GarageOperationsManagementSystem.Tests
{
    public class GarageServiceTests
    {
        private static Garage SampleGarage(string city = "Sofia", string address = "1 Main St")
            => new Garage { City = city, Address = address, Capacity = 10, WorkSchedule = "9-18" };

        private static (GarageService svc, Data.ApplicationDbContext ctx) Create(string dbName)
        {
            var ctx = TestDbContextFactory.Create(dbName);
            return (new GarageService(ctx), ctx);
        }

        // ── GetAllGaragesAsync ────────────────────────────────────────────────
        [Fact]
        public async Task GetAllGaragesAsync_ReturnsAllGarages()
        {
            var (svc, ctx) = Create(nameof(GetAllGaragesAsync_ReturnsAllGarages));
            ctx.Garages.AddRange(SampleGarage("Sofia"), SampleGarage("Plovdiv"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetAllGaragesAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllGaragesAsync_ReturnsEmpty_WhenNoGarages()
        {
            var (svc, _) = Create(nameof(GetAllGaragesAsync_ReturnsEmpty_WhenNoGarages));
            var result = await svc.GetAllGaragesAsync();
            Assert.Empty(result);
        }

        // ── GetGarageByIdAsync ────────────────────────────────────────────────
        [Fact]
        public async Task GetGarageByIdAsync_ReturnsGarage_WhenExists()
        {
            var (svc, ctx) = Create(nameof(GetGarageByIdAsync_ReturnsGarage_WhenExists));
            var garage = SampleGarage();
            ctx.Garages.Add(garage);
            await ctx.SaveChangesAsync();

            var result = await svc.GetGarageByIdAsync(garage.Id);

            Assert.NotNull(result);
            Assert.Equal("Sofia", result!.City);
        }

        [Fact]
        public async Task GetGarageByIdAsync_ReturnsNull_WhenNotFound()
        {
            var (svc, _) = Create(nameof(GetGarageByIdAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetGarageByIdAsync(999);
            Assert.Null(result);
        }

        // ── CreateGarageAsync ─────────────────────────────────────────────────
        [Fact]
        public async Task CreateGarageAsync_AddsGarage()
        {
            var (svc, ctx) = Create(nameof(CreateGarageAsync_AddsGarage));
            await svc.CreateGarageAsync(SampleGarage());
            Assert.Equal(1, ctx.Garages.Count());
        }

        // ── CreateGaragesAsync ────────────────────────────────────────────────
        [Fact]
        public async Task CreateGaragesAsync_AddsMultipleGarages()
        {
            var (svc, ctx) = Create(nameof(CreateGaragesAsync_AddsMultipleGarages));
            await svc.CreateGaragesAsync(new[] { SampleGarage("Sofia"), SampleGarage("Plovdiv") });
            Assert.Equal(2, ctx.Garages.Count());
        }

        // ── UpdateGarageAsync ─────────────────────────────────────────────────
        [Fact]
        public async Task UpdateGarageAsync_PersistsChanges()
        {
            var (svc, ctx) = Create(nameof(UpdateGarageAsync_PersistsChanges));
            var garage = SampleGarage();
            ctx.Garages.Add(garage);
            await ctx.SaveChangesAsync();

            garage.City = "Varna";
            await svc.UpdateGarageAsync(garage);

            Assert.Equal("Varna", ctx.Garages.Find(garage.Id)!.City);
        }

        // ── DeleteGarageAsync ─────────────────────────────────────────────────
        [Fact]
        public async Task DeleteGarageAsync_RemovesGarage()
        {
            var (svc, ctx) = Create(nameof(DeleteGarageAsync_RemovesGarage));
            var garage = SampleGarage();
            ctx.Garages.Add(garage);
            await ctx.SaveChangesAsync();

            await svc.DeleteGarageAsync(garage.Id);

            Assert.Empty(ctx.Garages);
        }

        [Fact]
        public async Task DeleteGarageAsync_DoesNotThrow_WhenNotFound()
        {
            var (svc, _) = Create(nameof(DeleteGarageAsync_DoesNotThrow_WhenNotFound));
            var ex = await Record.ExceptionAsync(() => svc.DeleteGarageAsync(999));
            Assert.Null(ex);
        }

        // ── GetQueryable ──────────────────────────────────────────────────────
        [Fact]
        public async Task GetQueryable_ReturnsQueryableOfGarages()
        {
            var (svc, ctx) = Create(nameof(GetQueryable_ReturnsQueryableOfGarages));
            ctx.Garages.Add(SampleGarage());
            await ctx.SaveChangesAsync();

            var query = svc.GetQueryable();

            Assert.Equal(1, query.Count());
        }
    }
}
