using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Admin,Employee")]
    public class HomeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IGarageService _garageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IEmployeeService employeeService, IGarageService garageService, UserManager<ApplicationUser> userManager)
        {
            _employeeService = employeeService;
            _garageService = garageService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var employee = userId != null
                ? await _employeeService.GetByUserIdAsync(userId)
                : null;
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGarage(int garageId, string city, string address, int capacity, string workSchedule)
        {
            var userId = _userManager.GetUserId(User);
            var employee = userId != null ? await _employeeService.GetByUserIdAsync(userId) : null;

            if (employee == null || !employee.IsTrusted || employee.GarageId != garageId)
            {
                TempData["ErrorMessage"] = "You do not have permission to edit this garage.";
                return RedirectToAction(nameof(Index));
            }

            var garage = await _garageService.GetGarageByIdAsync(garageId);
            if (garage == null)
            {
                TempData["ErrorMessage"] = "Garage not found.";
                return RedirectToAction(nameof(Index));
            }

            garage.City = city;
            garage.Address = address;
            garage.Capacity = capacity;
            garage.WorkSchedule = workSchedule;
            await _garageService.UpdateGarageAsync(garage);

            TempData["SuccessMessage"] = "Garage updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
