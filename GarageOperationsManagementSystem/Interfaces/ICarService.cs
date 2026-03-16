using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Interfaces
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllCarsAsync();

        Task<Car> GetCarByIdAsync(int Id);

        Task CreateCarAsync(Car car);

        Task DeleteCarAsync(int Id);
    }
}
