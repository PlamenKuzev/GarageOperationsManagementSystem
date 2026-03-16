using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IRepairOrderService
    {
        Task<IEnumerable<RepairOrder>> GetAllOrdersAsync();

        Task<RepairOrder> GetOrderByIdAsync(int Id);

        Task CreateOrderAsync(RepairOrder order);

        Task CompleteOrderAsync(int Id, decimal price);

    }
}
