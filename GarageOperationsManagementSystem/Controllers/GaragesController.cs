using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GaragesController : Controller
    {
        private readonly IGarageService _garageService;

        public GaragesController(IGarageService garageService)
        {
            _garageService = garageService;
        }

        public async Task<IActionResult> Index()
        {
            var garages = await _garageService.GetAllGaragesAsync();
            return View(garages);
        }

        [AllowAnonymous]
        public async Task<IActionResult> MapView()
        {
            var garages = await _garageService.GetAllGaragesAsync();
            return View(garages);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Garage garage)
        {
            if (!ModelState.IsValid)
            {
                return View(garage);
            }

            await _garageService.CreateGarageAsync(garage);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var garage = await _garageService.GetGarageByIdAsync(id);
            if (garage is null)
            {
                return NotFound();
            }

            return View(garage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Garage garage)
        {
            if (id != garage.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(garage);
            }

            await _garageService.UpdateGarageAsync(garage);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var garage = await _garageService.GetGarageByIdAsync(id);
            if (garage is null)
            {
                return NotFound();
            }

            return View(garage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _garageService.DeleteGarageAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoadDemoData()
        {
            var demoGarages = BuildDemoGarages(30);
            await _garageService.CreateGaragesAsync(demoGarages);
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
                var latitude = Math.Round(random.NextDouble() * (44.25 - 41.2) + 41.2, 6);
                var longitude = Math.Round(random.NextDouble() * (28.65 - 22.35) + 22.35, 6);

                garages.Add(new Garage
                {
                    City = cities[random.Next(cities.Length)],
                    Address = $"Demo Street {random.Next(1, 200)}, No. {i}",
                    Capacity = random.Next(4, 31),
                    WorkSchedule = schedules[random.Next(schedules.Length)],
                    Latitude = latitude,
                    Longitude = longitude
                });
            }

            return garages;
        }
    }
}
