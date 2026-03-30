using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IRepairOrderService
    {
        Task<IEnumerable<RepairOrder>> GetAllOrdersAsync();

        Task<RepairOrder> GetOrderByIdAsync(int Id);

        Task<RepairOrder?> GetOrderByIssueCodeAsync(string issueCode);

        Task CreateOrderAsync(RepairOrder order);

        Task CompleteOrderAsync(int Id, decimal price);

    }
}
