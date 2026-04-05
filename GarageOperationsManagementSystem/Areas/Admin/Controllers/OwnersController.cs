using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OwnersController : Controller
    {
        private readonly IOwnerService _ownerService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OwnersController(IOwnerService ownerService, UserManager<ApplicationUser> userManager)
        {
            _ownerService = ownerService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var owners = await _ownerService.GetAllAsync();
            return View(owners);
        }

        public async Task<IActionResult> Details(int id)
        {
            var owner = await _ownerService.GetByIdWithCarsAsync(id);
            if (owner == null) return NotFound();

            await PopulateLinkableUsersAsync(owner.ApplicationUserId);
            return View(owner);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FullName", "PhoneNumber", "Email")] Owner owner)
        {
            if (ModelState.IsValid)
            {
                await _ownerService.CreateAsync(owner);
                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var owner = await _ownerService.GetByIdAsync(id);
            if (owner == null) return NotFound();
            return View(owner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "FullName", "PhoneNumber", "Email")] Owner owner)
        {
            if (id != owner.Id) return BadRequest();

            if (ModelState.IsValid)
            {
                try
                {
                    await _ownerService.UpdateAsync(owner);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ownerService.ExistsAsync(owner.Id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var owner = await _ownerService.GetByIdWithCarsAsync(id);
            if (owner == null) return NotFound();
            return View(owner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _ownerService.ExistsAsync(id))
                return RedirectToAction(nameof(Index));

            if (await _ownerService.HasCarsAsync(id))
            {
                TempData["ErrorMessage"] = "Cannot delete an owner who still has cars. Remove or reassign cars first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            await _ownerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // ── Link / Unlink ─────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkUser(int id, string userId)
        {
            var owner = await _ownerService.GetByIdAsync(id);
            if (owner == null) return NotFound();

            owner.ApplicationUserId = userId;
            await _ownerService.UpdateAsync(owner);

            TempData["SuccessMessage"] = "Account linked successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlinkUser(int id)
        {
            var owner = await _ownerService.GetByIdAsync(id);
            if (owner == null) return NotFound();

            owner.ApplicationUserId = null;
            await _ownerService.UpdateAsync(owner);

            TempData["SuccessMessage"] = "Account unlinked.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task PopulateLinkableUsersAsync(string? currentUserId)
        {
            // All users that are not already linked to a different owner
            var allOwners = await _ownerService.GetAllAsync();
            var alreadyLinked = allOwners
                .Where(o => o.ApplicationUserId != null && o.ApplicationUserId != currentUserId)
                .Select(o => o.ApplicationUserId!)
                .ToHashSet();

            var users = _userManager.Users
                .OrderBy(u => u.Email)
                .ToList()
                .Where(u => !alreadyLinked.Contains(u.Id))
                .Select(u => new { u.Id, Label = u.Email ?? u.UserName ?? u.Id })
                .ToList();

            ViewData["LinkableUsers"] = new SelectList(users, "Id", "Label", currentUserId);
        }
    }
}
