using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Services.Implementations
{
    public class OwnerService : IOwnerService
    {
        private readonly ApplicationDbContext _context;

        public OwnerService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Owner>> GetAllAsync()
        {
            return await _context.Owners
                .AsNoTracking()
                .Include(o => o.Cars)
                .OrderBy(o => o.FullName)
                .ToListAsync();
        }

        public async Task<Owner?> GetByIdAsync(int id)
        {
            return await _context.Owners.FindAsync(id);
        }

        public async Task<Owner?> GetByIdWithCarsAsync(int id)
        {
            return await _context.Owners
                .Include(o => o.Cars)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task CreateAsync(Owner owner)
        {
            _context.Owners.Add(owner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Owner owner)
        {
            _context.Owners.Update(owner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner != null)
            {
                _context.Owners.Remove(owner);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Owners.AnyAsync(o => o.Id == id);
        }

        public async Task<bool> HasCarsAsync(int id)
        {
            return await _context.Cars.AnyAsync(c => c.OwnerId == id);
        }

        public IQueryable<Owner> GetQueryable()
        {
            return _context.Owners.AsNoTracking();
        }
    }
}
