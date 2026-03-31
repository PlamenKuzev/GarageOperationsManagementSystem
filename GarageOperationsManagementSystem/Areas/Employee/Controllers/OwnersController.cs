using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Admin,Employee")]
    public class OwnersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OwnersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var owners = await _context.Owners
                .AsNoTracking()
                .OrderBy(o => o.FullName)
                .ToListAsync();

            return View(owners);
        }

        public async Task<IActionResult> Details(int id)
        {
            var owner = await _context.Owners
                .AsNoTracking()
                .Include(o => o.Cars)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName", "PhoneNumber", "Email")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                _context.Add(owner);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var owner = await _context.Owners.FindAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "FullName", "PhoneNumber", "Email")] Owner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(owner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await OwnerExistsAsync(owner.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var owner = await _context.Owners
                .AsNoTracking()
                .Include(o => o.Cars)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var owner = await _context.Owners
                .Include(o => o.Cars)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (owner == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (owner.Cars.Count > 0)
            {
                TempData["ErrorMessage"] = "Cannot delete an owner who still has cars. Remove or reassign cars first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private Task<bool> OwnerExistsAsync(int id)
        {
            return _context.Owners.AnyAsync(e => e.Id == id);
        }
    }
}
