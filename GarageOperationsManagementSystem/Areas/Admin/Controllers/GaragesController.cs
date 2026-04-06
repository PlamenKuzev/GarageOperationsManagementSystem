using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Helpers;
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

        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            const int pageSize = 10;
            ViewData["CurrentFilter"] = searchString;
            ViewData["SearchPlaceholder"] = "Search by city or address…";

            var query = _garageService.GetQueryable().OrderBy(g => g.City);

            IQueryable<Garage> filtered = query;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                filtered = query.Where(g =>
                    g.City.Contains(searchString) || g.Address.Contains(searchString));
            }

            var paged = await PaginatedList<Garage>.CreateAsync(filtered, pageNumber, pageSize);
            ViewData["PageIndex"] = paged.PageIndex;
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["HasPreviousPage"] = paged.HasPreviousPage;
            ViewData["HasNextPage"] = paged.HasNextPage;

            return View(paged);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadDemoData()
        {
            var demoGarages = BuildDemoGarages(30);
            await _garageService.CreateGaragesAsync(demoGarages);
            TempData["SuccessMessage"] = "30 demo garages have been added.";
            return RedirectToAction(nameof(Index));
        }

        private static List<Garage> BuildDemoGarages(int count)
        {
            string[] cities =
            {
                "Sofia", "Plovdiv", "Varna", "Burgas", "Ruse",
                "Stara Zagora", "Pleven", "Sliven", "Dobrich", "Shumen",
                "Pernik", "Yambol", "Haskovo", "Pazardzhik", "Blagoevgrad"
            };

            string[] schedules =
            {
                "Mon-Fri 08:00-17:00",
                "Mon-Sat 09:00-18:00",
                "Mon-Fri 07:30-16:30",
                "Daily 10:00-19:00"
            };

            var random = new Random();
            var garages = new List<Garage>(count);

            for (var i = 1; i <= count; i++)
            {
                var latitude  = Math.Round(random.NextDouble() * (44.25 - 41.2) + 41.2, 6);
                var longitude = Math.Round(random.NextDouble() * (28.65 - 22.35) + 22.35, 6);

                garages.Add(new Garage
                {
                    City         = cities[random.Next(cities.Length)],
                    Address      = $"Demo Street {random.Next(1, 200)}, No. {i}",
                    Capacity     = random.Next(4, 31),
                    WorkSchedule = schedules[random.Next(schedules.Length)],
                    Latitude     = latitude,
                    Longitude    = longitude
                });
            }

            return garages;
        }
    }
}
