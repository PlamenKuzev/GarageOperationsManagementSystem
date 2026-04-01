using GarageOperationsManagementSystem.Interfaces;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GarageOperationsManagementSystem.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OwnersController : Controller
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        public async Task<IActionResult> Index()
        {
            var owners = await _ownerService.GetAllAsync();
            return View(owners);
        }

        public async Task<IActionResult> Details(int id)
        {
            var owner = await _ownerService.GetByIdWithCarsAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

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
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id", "FullName", "PhoneNumber", "Email")] Owner owner)
        {
            if (id != owner.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _ownerService.UpdateAsync(owner);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ownerService.ExistsAsync(owner.Id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(owner);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var owner = await _ownerService.GetByIdWithCarsAsync(id);
            if (owner == null)
            {
                return NotFound();
            }

            return View(owner);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _ownerService.ExistsAsync(id))
            {
                return RedirectToAction(nameof(Index));
            }

            if (await _ownerService.HasCarsAsync(id))
            {
                TempData["ErrorMessage"] = "Cannot delete an owner who still has cars. Remove or reassign cars first.";
                return RedirectToAction(nameof(Delete), new { id });
            }

            await _ownerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
