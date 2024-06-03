using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ObligatorioProgramacion3.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class AdministracionController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
    }
}
