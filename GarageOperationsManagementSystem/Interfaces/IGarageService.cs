using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface IGarageService 
    {
        Task<IEnumerable<Garage>> GetAllGaragesAsync();

        Task<Garage> GetGarageByIdAsync(int Id);

        Task CreateGarageAsync(Garage garage);

        Task DeleteGarageAsync(int Id);
    }
}
