using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace GarageOperationsManagementSystem.Services.Implementations
{
    public class RepairOrderService : IRepairOrderService
    {
        private readonly ApplicationDbContext _context;

        public RepairOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RepairOrder>> GetAllOrdersAsync()
        {
            return await _context.RepairOrders
                .Include(r => r.Car)
                .ThenInclude(c => c.Owner)
                .ToListAsync();
        }

        public async Task<RepairOrder> GetOrderByIdAsync(int id)
        {
            return await _context.RepairOrders
                .Include(r => r.Car)
                .ThenInclude(c => c.Owner)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task CreateOrderAsync(RepairOrder order)
        {
            _context.RepairOrders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task CompleteOrderAsync(int id, decimal price)
        {
            var order = await _context.RepairOrders.FindAsync(id);

            if (order != null)
            {
                order.IsCompleted = true;
                order.CompletionDate = DateTime.Now;
                order.RepairPrice = price;

                await _context.SaveChangesAsync();
            }
        }
    }
}
