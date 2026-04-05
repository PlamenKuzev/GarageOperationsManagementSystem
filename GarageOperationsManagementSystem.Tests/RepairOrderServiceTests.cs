using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.Services.Implementations;
using Xunit;

namespace GarageOperationsManagementSystem.Tests
{
    public class RepairOrderServiceTests
    {
        private static (RepairOrderService svc, Data.ApplicationDbContext ctx) Create(string dbName)
        {
            var ctx = TestDbContextFactory.Create(dbName);
            return (new RepairOrderService(ctx), ctx);
        }

        private static (int carId, int garageId) SeedCarAndGarage(Data.ApplicationDbContext ctx)
        {
            var owner  = new Owner  { FullName = "Alice", Email = "a@x.com", PhoneNumber = "1" };
            var garage = new Garage { City = "Sofia", Address = "1 Main St", Capacity = 5, WorkSchedule = "9-18" };
            ctx.Owners.Add(owner);
            ctx.Garages.Add(garage);
            ctx.SaveChanges();
            var car = new Car { Brand = "Toyota", Model = "Yaris", OwnerId = owner.Id };
            ctx.Cars.Add(car);
            ctx.SaveChanges();
            return (car.Id, garage.Id);
        }

        private static RepairOrder MakeOrder(int carId, int garageId, string code = "ABC123")
            => new RepairOrder
            {
                IssueCode = code,
                IssueDescription = "Test issue",
                ArrivalDate = DateTime.Now,
                CarId = carId,
                GarageId = garageId
            };

        [Fact]
        public async Task GetAllOrdersAsync_ReturnsAllOrders()
        {
            var (svc, ctx) = Create(nameof(GetAllOrdersAsync_ReturnsAllOrders));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            ctx.RepairOrders.AddRange(MakeOrder(carId, garageId, "AAA"), MakeOrder(carId, garageId, "BBB"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetAllOrdersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllOrdersAsync_IncludesCarAndGarage()
        {
            var (svc, ctx) = Create(nameof(GetAllOrdersAsync_IncludesCarAndGarage));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            ctx.RepairOrders.Add(MakeOrder(carId, garageId));
            await ctx.SaveChangesAsync();

            var order = (await svc.GetAllOrdersAsync()).First();

            Assert.NotNull(order.Car);
            Assert.NotNull(order.Car.Owner);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsOrder_WhenExists()
        {
            var (svc, ctx) = Create(nameof(GetOrderByIdAsync_ReturnsOrder_WhenExists));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            var order = MakeOrder(carId, garageId);
            ctx.RepairOrders.Add(order);
            await ctx.SaveChangesAsync();

            var result = await svc.GetOrderByIdAsync(order.Id);

            Assert.NotNull(result);
            Assert.Equal(order.IssueCode, result!.IssueCode);
        }

        [Fact]
        public async Task GetOrderByIdAsync_ReturnsNull_WhenNotFound()
        {
            var (svc, _) = Create(nameof(GetOrderByIdAsync_ReturnsNull_WhenNotFound));
            var result = await svc.GetOrderByIdAsync(999);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderByIssueCodeAsync_ReturnsOrder_WhenCodeMatches()
        {
            var (svc, ctx) = Create(nameof(GetOrderByIssueCodeAsync_ReturnsOrder_WhenCodeMatches));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            ctx.RepairOrders.Add(MakeOrder(carId, garageId, "XYZ999"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetOrderByIssueCodeAsync("XYZ999");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetOrderByIssueCodeAsync_ReturnsNull_WhenCodeNotFound()
        {
            var (svc, _) = Create(nameof(GetOrderByIssueCodeAsync_ReturnsNull_WhenCodeNotFound));
            var result = await svc.GetOrderByIssueCodeAsync("NOPE");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderByIssueCodeAsync_ReturnsNull_WhenCodeIsWhitespace()
        {
            var (svc, _) = Create(nameof(GetOrderByIssueCodeAsync_ReturnsNull_WhenCodeIsWhitespace));
            var result = await svc.GetOrderByIssueCodeAsync("   ");
            Assert.Null(result);
        }

        [Fact]
        public async Task GetOrderByIssueCodeAsync_TrimsWhitespace()
        {
            var (svc, ctx) = Create(nameof(GetOrderByIssueCodeAsync_TrimsWhitespace));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            ctx.RepairOrders.Add(MakeOrder(carId, garageId, "TRIM01"));
            await ctx.SaveChangesAsync();

            var result = await svc.GetOrderByIssueCodeAsync("  TRIM01  ");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateOrderAsync_AddsOrder()
        {
            var (svc, ctx) = Create(nameof(CreateOrderAsync_AddsOrder));
            var (carId, garageId) = SeedCarAndGarage(ctx);

            await svc.CreateOrderAsync(MakeOrder(carId, garageId));

            Assert.Equal(1, ctx.RepairOrders.Count());
        }

        [Fact]
        public async Task CompleteOrderAsync_SetsCompletedAndPrice()
        {
            var (svc, ctx) = Create(nameof(CompleteOrderAsync_SetsCompletedAndPrice));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            var order = MakeOrder(carId, garageId);
            ctx.RepairOrders.Add(order);
            await ctx.SaveChangesAsync();

            await svc.CompleteOrderAsync(order.Id, 250m);

            var updated = ctx.RepairOrders.Find(order.Id)!;
            Assert.True(updated.IsCompleted);
            Assert.Equal(250m, updated.RepairPrice);
            Assert.NotNull(updated.CompletionDate);
        }

        [Fact]
        public async Task CompleteOrderAsync_DoesNotThrow_WhenOrderNotFound()
        {
            var (svc, _) = Create(nameof(CompleteOrderAsync_DoesNotThrow_WhenOrderNotFound));
            var ex = await Record.ExceptionAsync(() => svc.CompleteOrderAsync(999, 100m));
            Assert.Null(ex);
        }

        [Fact]
        public async Task UpdateOrderAsync_PersistsChanges()
        {
            var (svc, ctx) = Create(nameof(UpdateOrderAsync_PersistsChanges));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            var order = MakeOrder(carId, garageId);
            ctx.RepairOrders.Add(order);
            await ctx.SaveChangesAsync();

            order.IssueDescription = "Updated description";
            await svc.UpdateOrderAsync(order);

            Assert.Equal("Updated description", ctx.RepairOrders.Find(order.Id)!.IssueDescription);
        }

        [Fact]
        public async Task DeleteOrderAsync_RemovesOrder()
        {
            var (svc, ctx) = Create(nameof(DeleteOrderAsync_RemovesOrder));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            var order = MakeOrder(carId, garageId);
            ctx.RepairOrders.Add(order);
            await ctx.SaveChangesAsync();

            await svc.DeleteOrderAsync(order.Id);

            Assert.Empty(ctx.RepairOrders);
        }

        [Fact]
        public async Task DeleteOrderAsync_DoesNotThrow_WhenNotFound()
        {
            var (svc, _) = Create(nameof(DeleteOrderAsync_DoesNotThrow_WhenNotFound));
            var ex = await Record.ExceptionAsync(() => svc.DeleteOrderAsync(999));
            Assert.Null(ex);
        }

        [Fact]
        public async Task GetQueryable_ReturnsQueryableOfOrders()
        {
            var (svc, ctx) = Create(nameof(GetQueryable_ReturnsQueryableOfOrders));
            var (carId, garageId) = SeedCarAndGarage(ctx);
            ctx.RepairOrders.Add(MakeOrder(carId, garageId));
            await ctx.SaveChangesAsync();

            var query = svc.GetQueryable();

            Assert.Equal(1, query.Count());
        }
    }
}
