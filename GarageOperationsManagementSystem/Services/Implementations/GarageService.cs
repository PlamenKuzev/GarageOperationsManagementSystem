using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Services.Implementations
{
    public class GarageService : IGarageService
    {
        private readonly ApplicationDbContext _context;

        public GarageService(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<Garage>> GetAllGaragesAsync()
        {
            return await _context.Garages.ToListAsync();
        }

        public async Task<Garage?> GetGarageByIdAsync(int id)
        {
            return await _context.Garages.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task CreateGarageAsync(Garage garage)
        {
            _context.Garages.Add(garage);
            await _context.SaveChangesAsync();
        }

        public async Task CreateGaragesAsync(IEnumerable<Garage> garages)
        {
            _context.Garages.AddRange(garages);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGarageAsync(Garage garage)
        {
            _context.Garages.Update(garage);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGarageAsync(int id)
        {
            var garage = await _context.Garages.FindAsync(id);

            if (garage != null)
            {
                _context.Garages.Remove(garage);
                await _context.SaveChangesAsync();
            }
        }

    }
}
