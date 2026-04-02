using GarageOperationsManagementSystem.Helpers;
using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Admin,Employee")]
    public class RepairOrdersController : Controller
    {
        private readonly IRepairOrderService _repairOrderService;
        private readonly ICarService _carService;
        private readonly IGarageService _garageService;
        private readonly IEmployeeService _employeeService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RepairOrdersController(
            IRepairOrderService repairOrderService,
            ICarService carService,
            IGarageService garageService,
            IEmployeeService employeeService,
            UserManager<ApplicationUser> userManager)
        {
            _repairOrderService = repairOrderService;
            _carService = carService;
            _garageService = garageService;
            _employeeService = employeeService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            const int pageSize = 10;
            var (emp, isTrusted) = await GetCurrentEmployeeAsync();
            ViewData["IsTrusted"] = isTrusted;
            ViewData["CurrentFilter"] = searchString;
            ViewData["SearchPlaceholder"] = "Search by issue code…";

            var query = _repairOrderService.GetQueryable()
                .Include(r => r.Car).ThenInclude(c => c!.Owner)
                .Include(r => r.Garage)
                .OrderByDescending(r => r.ArrivalDate);

            IQueryable<RepairOrder> filtered = query;
            if (!isTrusted && emp != null)
            {
                filtered = filtered.Where(r => r.GarageId == emp.GarageId);
            }
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                filtered = filtered.Where(r => r.IssueCode.Contains(searchString));
            }

            var paged = await PaginatedList<RepairOrder>.CreateAsync(filtered, pageNumber, pageSize);
            ViewData["PageIndex"] = paged.PageIndex;
            ViewData["TotalPages"] = paged.TotalPages;
            ViewData["HasPreviousPage"] = paged.HasPreviousPage;
            ViewData["HasNextPage"] = paged.HasNextPage;

            return View(paged);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Create()
        {
            await PopulateRepairOrderSelectsAsync();
            return View(new RepairOrder { ArrivalDate = DateTime.Now });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(nameof(RepairOrder.IssueCode), nameof(RepairOrder.IssueDescription), nameof(RepairOrder.ArrivalDate),
                nameof(RepairOrder.CompletionDate), nameof(RepairOrder.IsCompleted), nameof(RepairOrder.RepairPrice),
                nameof(RepairOrder.CarId), nameof(RepairOrder.GarageId))]
            RepairOrder order)
        {
            if (string.IsNullOrWhiteSpace(order.IssueCode))
            {
                order.IssueCode = Guid.NewGuid().ToString("N")[..8].ToUpper();
                ModelState.Remove(nameof(RepairOrder.IssueCode));
            }

            if (!ModelState.IsValid)
            {
                await PopulateRepairOrderSelectsAsync(order.CarId, order.GarageId);
                return View(order);
            }

            await _repairOrderService.CreateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var (emp, isTrusted) = await GetCurrentEmployeeAsync();
            if (!isTrusted && emp != null && order.GarageId != emp.GarageId)
            {
                TempData["ErrorMessage"] = "You can only edit repair orders belonging to your garage.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateRepairOrderSelectsAsync(order.CarId, order.GarageId);
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(nameof(RepairOrder.Id), nameof(RepairOrder.IssueCode), nameof(RepairOrder.IssueDescription),
                nameof(RepairOrder.ArrivalDate), nameof(RepairOrder.CompletionDate), nameof(RepairOrder.IsCompleted),
                nameof(RepairOrder.RepairPrice), nameof(RepairOrder.CarId), nameof(RepairOrder.GarageId))]
            RepairOrder order)
        {
            if (id != order.Id) return BadRequest();

            var (emp, isTrusted) = await GetCurrentEmployeeAsync();
            if (!isTrusted && emp != null && order.GarageId != emp.GarageId)
            {
                TempData["ErrorMessage"] = "You can only edit repair orders belonging to your garage.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                await PopulateRepairOrderSelectsAsync(order.CarId, order.GarageId);
                return View(order);
            }

            await _repairOrderService.UpdateOrderAsync(order);
            TempData["SuccessMessage"] = "Repair order updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var (emp, isTrusted) = await GetCurrentEmployeeAsync();
            if (!isTrusted && emp != null && order.GarageId != emp.GarageId)
            {
                TempData["ErrorMessage"] = "You can only delete repair orders belonging to your garage.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            var (emp, isTrusted) = await GetCurrentEmployeeAsync();
            if (!isTrusted && emp != null && order.GarageId != emp.GarageId)
            {
                TempData["ErrorMessage"] = "You can only delete repair orders belonging to your garage.";
                return RedirectToAction(nameof(Index));
            }

            await _repairOrderService.DeleteOrderAsync(id);
            TempData["SuccessMessage"] = "Repair order deleted.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id, decimal repairPrice)
        {
            if (repairPrice < 0)
            {
                TempData["ErrorMessage"] = "Repair price cannot be negative.";
                return RedirectToAction(nameof(Details), new { id });
            }

            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _repairOrderService.CompleteOrderAsync(id, repairPrice);
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<(Models.Employee? emp, bool isTrusted)> GetCurrentEmployeeAsync()
        {
            var userId = _userManager.GetUserId(User);
            var emp = userId != null ? await _employeeService.GetByUserIdAsync(userId) : null;
            return (emp, emp?.IsTrusted ?? false);
        }

        private async Task PopulateRepairOrderSelectsAsync(int? selectedCarId = null, int? selectedGarageId = null)
        {
            var cars = (await _carService.GetAllCarsAsync())
                .OrderBy(c => c.Brand).ThenBy(c => c.Model);

            var carDisplay = cars
                .Select(c => new { c.Id, Label = $"{c.Brand} {c.Model} (Owner: {c.Owner?.FullName ?? "—"})" })
                .ToList();

            ViewData["CarId"] = new SelectList(carDisplay, nameof(Car.Id), "Label", selectedCarId);

            var garages = (await _garageService.GetAllGaragesAsync())
                .OrderBy(g => g.City).ThenBy(g => g.Address);

            var garageDisplay = garages
                .Select(g => new { g.Id, Label = $"{g.City} — {g.Address}" })
                .ToList();

            ViewData["GarageId"] = new SelectList(garageDisplay, nameof(Garage.Id), "Label", selectedGarageId);
        }
    }
}
