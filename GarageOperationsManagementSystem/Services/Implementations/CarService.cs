using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace GarageOperationsManagementSystem.Services.Implementations
{
    public class CarService : ICarService
    {
        private readonly ApplicationDbContext _context;

        public CarService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<Car>> GetAllCarsAsync()
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .ToListAsync();
        }

        public async Task<Car?> GetCarByIdAsync(int id)
        {
            return await _context.Cars
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task CreateCarAsync(Car car)
        {
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCarAsync(int id)
        {
            var car = await _context.Cars.FindAsync(id);

            if (car != null)
            {
                _context.Cars.Remove(car);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateCarAsync(Car car)
        {
            _context.Cars.Update(car);
            await _context.SaveChangesAsync();
        }

    }
}
