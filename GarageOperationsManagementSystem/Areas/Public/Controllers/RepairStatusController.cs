using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.ViewModels.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Areas.Public.Controllers
{
    [Area("Public")]
    [AllowAnonymous]
    public class RepairStatusController : Controller
    {
        private readonly IRepairOrderService _repairOrderService;

        public RepairStatusController(IRepairOrderService repairOrderService)
        {
            _repairOrderService = repairOrderService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new RepairStatusLookupViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RepairStatusLookupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var order = await _repairOrderService.GetOrderByIssueCodeAsync(model.IssueCode);
            if (order is null)
            {
                model.NotFound = true;
                model.Result = null;
                return View(model);
            }

            model.NotFound = false;
            model.Result = new RepairStatusResultViewModel
            {
                IssueCode = order.IssueCode,
                IsCompleted = order.IsCompleted,
                ArrivalDate = order.ArrivalDate,
                CompletionDate = order.CompletionDate,
                RepairPrice = order.RepairPrice,
                GarageCity = order.Garage?.City ?? string.Empty,
                GarageAddress = order.Garage?.Address ?? string.Empty,
                CarSummary = $"{order.Car?.Brand} {order.Car?.Model}".Trim()
            };

            return View(model);
        }
    }
}

