using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Areas.Employee.Controllers
{
    [Area("Employee")]
    [Authorize(Roles = "Admin,Employee")]
    public class RepairOrdersController : Controller
    {
        private readonly IRepairOrderService _repairService;

        public RepairOrdersController(IRepairOrderService repairService)
        {
            _repairService = repairService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _repairService.GetAllOrdersAsync();
            return View(orders);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RepairOrder order)
        {
            if (ModelState.IsValid)
            {
                order.ArrivalDate = DateTime.Now;
                order.IssueCode = Guid.NewGuid().ToString().Substring(0, 8);

                await _repairService.CreateOrderAsync(order);

                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        public async Task<IActionResult> Complete(int id)
        {
            await _repairService.CompleteOrderAsync(id, 100);
            return RedirectToAction(nameof(Index));
        }
    }
}
