using GarageOperationsManagementSystem.Data;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly ApplicationDbContext _context;

        public CarsController(ICarService carService, ApplicationDbContext context)
        {
            _carService = carService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateOwnerSelectAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Brand", "Model", "Year", "Mileage", "OwnerId")] Car car)
        {
            if (ModelState.IsValid)
            {
                await _carService.CreateCarAsync(car);
                return RedirectToAction(nameof(Index));
            }

            await PopulateOwnerSelectAsync(car.OwnerId);
            return View(car);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            await PopulateOwnerSelectAsync(car.OwnerId);
            return View(car);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "Brand", "Model", "Year", "Mileage", "OwnerId")] Car car)
        {
            if (id != car.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _carService.UpdateCarAsync(car);
                return RedirectToAction(nameof(Index));
            }

            await PopulateOwnerSelectAsync(car.OwnerId);
            return View(car);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteCarAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateOwnerSelectAsync(int? selectedOwnerId = null)
        {
            var owners = await _context.Owners
                .OrderBy(o => o.FullName)
                .ToListAsync();

            ViewData["OwnerId"] = new SelectList(owners, "Id", "FullName", selectedOwnerId);
        }
    }
}