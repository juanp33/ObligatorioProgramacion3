using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ObligatorioProgramacion3.Models;
using System.Diagnostics;

namespace ObligatorioProgramacion3.Controllers
{
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult AccesoDenegado()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Error");
            }
            else
            {
                return RedirectToAction("Login", "Usuarios");
            }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
     

        public IActionResult Reseñas() {
            return View();
        }
    }
}
