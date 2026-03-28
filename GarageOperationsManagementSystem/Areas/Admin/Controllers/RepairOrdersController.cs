using GarageOperationsManagementSystem.Data;
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
        private readonly ApplicationDbContext _context;
        public RepairOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.RepairOrders
                .Include(r => r.Car).ThenInclude(c => c.Owner)
                .Include(r => r.Garage)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.RepairOrders
                .Include(r => r.Car).ThenInclude(c => c.Owner)
                .Include(r => r.Garage)
                .FirstOrDefaultAsync(r => r.Id == id);

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

            _context.RepairOrders.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.RepairOrders.FindAsync(id);
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
            _context.Entry(order).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.RepairOrders
                .Include(r => r.Car).ThenInclude(c => c.Owner)
                .Include(r => r.Garage)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.RepairOrders.FindAsync(id);
            if (order == null) return NotFound();
            _context.RepairOrders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateRepairOrderSelectsAsync(int? selectedCarId = null, int? selectedGarageId = null)
        {
            var cars = await _context.Cars
                .Include(c => c.Owner)
                .OrderBy(c => c.Brand)
                .ThenBy(c => c.Model)
                .ToListAsync();

            var carDisplay = cars
                .Select(c => new { c.Id, Label = $"{c.Brand} {c.Model} (Owner: {c.Owner?.FullName ?? "—"})" })
                .ToList();

            ViewData["CarId"] = new SelectList(carDisplay, nameof(Car.Id), "Label", selectedCarId);

            var garages = await _context.Garages.OrderBy(g => g.City).ThenBy(g => g.Address).ToListAsync();
            var garageDisplay = garages
                .Select(g => new { g.Id, Label = $"{g.City} — {g.Address}" })
                .ToList();

            ViewData["GarageId"] = new SelectList(garageDisplay, nameof(Garage.Id), "Label", selectedGarageId);
        }
    }
}
