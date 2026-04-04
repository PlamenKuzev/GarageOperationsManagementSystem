using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using GarageOperationsManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CarsController : Controller
    {
        private readonly ICarService _carService;
        private readonly IOwnerService _ownerService;

        public CarsController(ICarService carService, IOwnerService ownerService)
        {
            _carService = carService;
            _ownerService = ownerService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllCarsAsync();
            return View(cars);
        }

        public async Task<IActionResult> Details(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        public async Task<IActionResult> Create()
        {
            if (!await _ownerService.ExistsAsync(0) && !(await _ownerService.GetAllAsync()).Any())
            {
                TempData["ErrorMessage"] = "Add at least one owner (Admin → Owners → Create) before creating a car.";
            }

            await PopulateOwnerSelectAsync();
            return View(new AdminCarFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCarFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateOwnerSelectAsync(vm.OwnerId);
                return View(vm);
            }

            var car = new Car
            {
                Brand = vm.Brand,
                Model = vm.Model,
                Year = vm.Year,
                Mileage = vm.Mileage,
                OwnerId = vm.OwnerId!.Value,
                Owner = null!
            };

            await _carService.CreateCarAsync(car);
            TempData["SuccessMessage"] = $"{car.Brand} {car.Model} added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();

            await PopulateOwnerSelectAsync(car.OwnerId);
            return View(new AdminCarFormViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                Mileage = car.Mileage,
                OwnerId = car.OwnerId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCarFormViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateOwnerSelectAsync(vm.OwnerId);
                return View(vm);
            }

            await _carService.UpdateCarAsync(new Car
            {
                Id = vm.Id,
                Brand = vm.Brand,
                Model = vm.Model,
                Year = vm.Year,
                Mileage = vm.Mileage,
                OwnerId = vm.OwnerId!.Value,
                Owner = null!
            });

            TempData["SuccessMessage"] = "Car updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var car = await _carService.GetCarByIdAsync(id);
            if (car == null) return NotFound();
            return View(car);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _carService.DeleteCarAsync(id);
            TempData["SuccessMessage"] = "Car deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateOwnerSelectAsync(int? selectedOwnerId = null)
        {
            var owners = (await _ownerService.GetAllAsync()).OrderBy(o => o.FullName).ToList();
            ViewData["OwnerId"] = new SelectList(owners, nameof(Owner.Id), nameof(Owner.FullName), selectedOwnerId);
            ViewData["OwnerCount"] = owners.Count;
        }
    }
}
