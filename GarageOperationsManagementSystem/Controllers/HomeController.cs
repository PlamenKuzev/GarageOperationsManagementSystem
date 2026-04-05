using System.Diagnostics;
using GarageOperationsManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace GarageOperationsManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            if (User.IsInRole("Employee"))
                return RedirectToAction("Index", "Home", new { area = "Employee" });

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult NotFound404()
        {
            Response.StatusCode = 404;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ServerError()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}
