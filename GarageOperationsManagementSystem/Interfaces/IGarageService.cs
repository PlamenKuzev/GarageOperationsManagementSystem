using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IGarageService 
    {
        Task<IEnumerable<Garage>> GetAllGaragesAsync();

        Task<Garage?> GetGarageByIdAsync(int id);

        Task CreateGarageAsync(Garage garage);

        Task CreateGaragesAsync(IEnumerable<Garage> garages);

        Task UpdateGarageAsync(Garage garage);

        Task DeleteGarageAsync(int id);

        IQueryable<Garage> GetQueryable();
    }
}
