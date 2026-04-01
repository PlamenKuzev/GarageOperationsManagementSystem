using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.ViewModels.Admin;
using EmployeeModel = GarageOperationsManagementSystem.Models.Employee;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IGarageService _garageService;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeesController(
            IEmployeeService employeeService,
            IGarageService garageService,
            UserManager<ApplicationUser> userManager)
        {
            _employeeService = employeeService;
            _garageService = garageService;
            _userManager = userManager;
        }

        // GET: Admin/Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllAsync();
            var viewModels = employees.Select(e => new EmployeeIndexViewModel
            {
                Id = e.Id,
                Name = e.Name,
                Position = e.Position,
                Salary = e.Salary,
                WorkingSince = e.WorkingSince,
                GarageName = e.Garage?.City ?? "—",
                IsTrusted = e.IsTrusted,
                LinkedEmail = e.ApplicationUser?.Email
            });
            return View(viewModels);
        }

        // GET: Admin/Employees/Create
        public async Task<IActionResult> Create()
        {
            await PopulateGarageSelectAsync();
            return View(new CreateEmployeeViewModel { WorkingSince = DateTime.Today });
        }

        // POST: Admin/Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmployeeViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateGarageSelectAsync(vm.GarageId);
                return View(vm);
            }

            // 1. Create the ApplicationUser account
            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FullName = vm.Name,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                await PopulateGarageSelectAsync(vm.GarageId);
                return View(vm);
            }

            await _userManager.AddToRoleAsync(user, "Employee");

            // 2. Create the linked Employee record
            var employee = new EmployeeModel
            {
                Name = vm.Name,
                Position = vm.Position,
                Salary = vm.Salary,
                WorkingSince = vm.WorkingSince,
                GarageId = vm.GarageId,
                IsTrusted = vm.IsTrusted,
                ApplicationUserId = user.Id
            };

            try
            {
                await _employeeService.CreateAsync(employee);
            }
            catch
            {
                // Roll back the user if the employee record fails
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError(string.Empty, "Failed to create employee record. Please try again.");
                await PopulateGarageSelectAsync(vm.GarageId);
                return View(vm);
            }

            TempData["SuccessMessage"] = $"Employee \"{vm.Name}\" created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Employees/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            await PopulateGarageSelectAsync(employee.GarageId);
            return View(new EditEmployeeViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Position = employee.Position,
                Salary = employee.Salary,
                WorkingSince = employee.WorkingSince,
                GarageId = employee.GarageId,
                IsTrusted = employee.IsTrusted
            });
        }

        // POST: Admin/Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditEmployeeViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateGarageSelectAsync(vm.GarageId);
                return View(vm);
            }

            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            employee.Name = vm.Name;
            employee.Position = vm.Position;
            employee.Salary = vm.Salary;
            employee.WorkingSince = vm.WorkingSince;
            employee.GarageId = vm.GarageId;
            employee.IsTrusted = vm.IsTrusted;

            await _employeeService.UpdateAsync(employee);

            TempData["SuccessMessage"] = $"Employee \"{employee.Name}\" updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Employees/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Admin/Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();

            var name = employee.Name;

            // Delete linked ApplicationUser if present
            if (employee.ApplicationUserId != null)
            {
                var user = await _userManager.FindByIdAsync(employee.ApplicationUserId);
                if (user != null)
                    await _userManager.DeleteAsync(user);
            }

            await _employeeService.DeleteAsync(id);

            TempData["SuccessMessage"] = $"Employee \"{name}\" and their account have been deleted.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateGarageSelectAsync(int? selectedId = null)
        {
            var garages = await _garageService.GetAllGaragesAsync();
            ViewData["GarageId"] = new SelectList(garages, "Id", "City", selectedId);
        }
    }
}
