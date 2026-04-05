using GarageOperationsManagementSystem.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GarageOperationsManagementSystem.Models;

namespace GarageOperationsManagementSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IOwnerService _ownerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(IOwnerService ownerService, UserManager<ApplicationUser> userManager)
        {
            _ownerService = ownerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null) return Challenge();

            var owner = await _ownerService.GetByUserIdAsync(userId);
            return View(owner);
        }
    }
}
