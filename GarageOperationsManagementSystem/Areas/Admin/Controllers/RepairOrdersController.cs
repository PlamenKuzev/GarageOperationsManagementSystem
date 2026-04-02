using GarageOperationsManagementSystem.Helpers;
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
    public class RepairOrdersController : Controller
    {
        private readonly IRepairOrderService _repairOrderService;
        private readonly ICarService _carService;
        private readonly IGarageService _garageService;

        public RepairOrdersController(
            IRepairOrderService repairOrderService,
            ICarService carService,
            IGarageService garageService)
        {
            _repairOrderService = repairOrderService;
            _carService = carService;
            _garageService = garageService;
        }

        public async Task<IActionResult> Index(string? searchString, int pageNumber = 1)
        {
            const int pageSize = 10;
            ViewData["CurrentFilter"] = searchString;
            ViewData["SearchPlaceholder"] = "Search by issue code or status…";

            var query = _repairOrderService.GetQueryable()
                .Include(r => r.Car).ThenInclude(c => c!.Owner)
                .Include(r => r.Garage)
                .OrderByDescending(r => r.ArrivalDate);

            IQueryable<RepairOrder> filtered = query;
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var lower = searchString.Trim().ToLower();
                filtered = query.Where(r =>
                    r.IssueCode.Contains(searchString) ||
                    (lower == "done" && r.IsCompleted) ||
                    (lower == "in progress" && !r.IsCompleted));
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
            if (order == null)
            {
                return NotFound();
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
            if (!ModelState.IsValid)
            {
                await PopulateRepairOrderSelectsAsync(order.CarId, order.GarageId);
                return View(order);
            }

            await _repairOrderService.UpdateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _repairOrderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _repairOrderService.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
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
