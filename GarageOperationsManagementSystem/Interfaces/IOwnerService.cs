using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IOwnerService
    {
        Task<IEnumerable<Owner>> GetAllAsync();
        Task<Owner?> GetByIdAsync(int id);
        Task<Owner?> GetByIdWithCarsAsync(int id);
        Task CreateAsync(Owner owner);
        Task UpdateAsync(Owner owner);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> HasCarsAsync(int id);
        IQueryable<Owner> GetQueryable();
    }
}
