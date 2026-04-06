using GarageOperationsManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Controllers
{
    public class GaragesController : Controller
    {
        private readonly IGarageService _garageService;

        public GaragesController(IGarageService garageService)
        {
            _garageService = garageService;
        }

        [AllowAnonymous]
        public async Task<IActionResult> MapView()
        {
            var garages = await _garageService.GetAllGaragesAsync();
            return View(garages);
        }
    }
}
