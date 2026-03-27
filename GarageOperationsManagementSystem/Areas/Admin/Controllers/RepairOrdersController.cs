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
            ViewData["CarId"] = new SelectList(
                await _context.Cars.Include(c => c.Owner).ToListAsync(),
                "Id",
                "Id"
            );
            ViewData["GarageId"] = new SelectList(await _context.Garages.ToListAsync(), "Id", "Id");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepairOrder order)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CarId"] = new SelectList(await _context.Cars.ToListAsync(), "Id", "Id", order.CarId);
                ViewData["GarageId"] = new SelectList(await _context.Garages.ToListAsync(), "Id", "Id", order.GarageId);
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

            ViewData["CarId"] = new SelectList(await _context.Cars.ToListAsync(), "Id", "Id", order.CarId);
            ViewData["GarageId"] = new SelectList(await _context.Garages.ToListAsync(), "Id", "Id", order.GarageId);
            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RepairOrder order)
        {
            if (id != order.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewData["CarId"] = new SelectList(await _context.Cars.ToListAsync(), "Id", "Id", order.CarId);
                ViewData["GarageId"] = new SelectList(await _context.Garages.ToListAsync(), "Id", "Id", order.GarageId);
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
    }
}
