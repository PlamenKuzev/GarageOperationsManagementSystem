using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GaragesController : Controller
    {
        private readonly IGarageService _garageService;
        private readonly ApplicationDbContext _context;

        public GaragesController(IGarageService garageService, ApplicationDbContext context)
        {
            _garageService = garageService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllGaragesAsync();
            return View(garages.OrderBy(g => g.City).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var garage = await _garageService.GetGarageByIdAsync(id);
            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("City", "Address", "Capacity", "WorkSchedule", "Latitude", "Longitude")]
            Garage garage)
        {
            if (ModelState.IsValid)
            {
                await _garageService.CreateGarageAsync(garage);
                return RedirectToAction(nameof(Index));
            }

            return View(garage);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var garage = await _garageService.GetGarageByIdAsync(id);
            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id", "City", "Address", "Capacity", "WorkSchedule", "Latitude", "Longitude")]
            Garage incoming)
        {
            if (id != incoming.Id)
            {
                return BadRequest();
            }

            var existing = await _garageService.GetGarageByIdAsync(id);
            if (existing == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existing.City = incoming.City;
                existing.Address = incoming.Address;
                existing.Capacity = incoming.Capacity;
                existing.WorkSchedule = incoming.WorkSchedule;
                existing.Latitude = incoming.Latitude;
                existing.Longitude = incoming.Longitude;

                await _garageService.UpdateGarageAsync(existing);
                return RedirectToAction(nameof(Index));
            }

            return View(incoming);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var garage = await _context.Garages
                .AsNoTracking()
                .Include(g => g.RepairOrders)
                .Include(g => g.Employees)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null)
            {
                return NotFound();
            }

            return View(garage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var garage = await _context.Garages
                .Include(g => g.RepairOrders)
                .Include(g => g.Employees)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (garage == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (garage.RepairOrders.Count > 0 || garage.Employees.Count > 0)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete a garage that still has repair orders or employees. Remove them first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            await _garageService.DeleteGarageAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
