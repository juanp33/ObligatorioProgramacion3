using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;

namespace ObligatorioProgramacion3.Controllers
{
    public class CarritoController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        private readonly CarritoService _carritoService;

        public CarritoController(CarritoService carritoService, ObligatorioProgramacion3Context context)
        {

            _carritoService = carritoService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var platos = await _context.Platos.ToListAsync(); // Implementa este método para obtener todos los platos
            return View(platos);
        }

        [HttpPost]
        public async Task<IActionResult> AñadirAlCarrito(int id)
        {
            var plato = await _context.Platos.FirstOrDefaultAsync(p => p.Id == id); // Implementa este método para obtener los detalles del Plato por id
            var carritoItem = new CarritoItem
            {
                PlatoId = plato.Id,
                NombrePlato = plato.NombrePlato,
                Descripción = plato.Descripcion,
                Precio = plato.Precio,
                Cantidad = 1
            };
            _carritoService.AñadirAlCarrito(carritoItem);
            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult EliminarDelCarrito(int id)
        {
            _carritoService.EliminarDelCarrito(id);
            return Json(new { success = true });
        }

        public IActionResult ObtenerCarrito()
        {
            var carritoItems = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();
            return PartialView("_CarritoPartial", new CarritoViewModel { Items = carritoItems, Total = total });
        }
    }
}
