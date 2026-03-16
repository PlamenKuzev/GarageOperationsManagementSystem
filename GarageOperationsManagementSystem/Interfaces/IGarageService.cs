using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IGarageService 
    {
        Task<IEnumerable<Garage>> GetAllGaragesAsync();

        Task<Garage> GetGaragesByIdAsync(int Id);

        Task CreateGarageAsync(Garage garage);

        Task DeleteGarageAsync(int Id);
    }
}
