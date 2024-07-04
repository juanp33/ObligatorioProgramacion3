using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;
using System.Security.Claims;


namespace ObligatorioProgramacion3.Controllers
{
    
    public class OrdenesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;
        private readonly CarritoService _carritoService;

        public OrdenesController(ObligatorioProgramacion3Context context, CarritoService carritoService)
        {
            _context = context;
            _carritoService = carritoService;
        }


        public async Task<IActionResult> GenerarOrden(int reservaId)
        {

            var items = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();
            var ordenExiste = _context.Ordenes.Any(o => o.ReservaId == reservaId);

            if (ordenExiste)
            {
                var ordenYaExistente = await _context.Ordenes.FirstOrDefaultAsync(o => o.ReservaId == reservaId);

                
                var ordenDetalles = await _context.OrdenDetalles.Where(od => od.OrdenId == ordenYaExistente.Id).ToListAsync();

               
                _context.OrdenDetalles.RemoveRange(ordenDetalles);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    OrdenDetalle ordenDetalle = new OrdenDetalle
                    {
                        OrdenId = ordenYaExistente.Id,
                        PlatoId = item.PlatoId,
                        Cantidad = item.Cantidad,

                    };
                    _context.OrdenDetalles.Add(ordenDetalle);


                }
                ordenYaExistente.Total = total;
                _context.Ordenes.Update(ordenYaExistente);
                await _context.SaveChangesAsync();
               
            }
            else
            {

                Ordene orden = new Ordene
                {
                    ReservaId = reservaId,
                    Total = _carritoService.ObtenerTotal()
                };

                _context.Ordenes.Add(orden);
                await _context.SaveChangesAsync();

                foreach (var item in items)
                {
                    OrdenDetalle ordenDetalle = new OrdenDetalle
                    {
                        OrdenId = orden.Id,
                        PlatoId = item.PlatoId,
                        Cantidad = item.Cantidad,

                    };
                    _context.OrdenDetalles.Add(ordenDetalle);
                    await _context.SaveChangesAsync();
                }
                
            }
            _carritoService.LimpiarCarrito();

            return RedirectToAction("MostrarOrdenes");
        }


        public async Task<IActionResult> MostrarOrdenes()
        {
            var userClaims = User.Claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();

            var ordenesSinPago = await _context.Ordenes
                .Include(o => o.Reserva) 
                .Where(o => !_context.Pagos.Any(p => p.ReservaId == o.ReservaId))
                .ToListAsync();

            ViewBag.UserClaims = userClaims;

            return View(ordenesSinPago);
        }
        // GET: Ordenes
        [Authorize(Policy = "OrdenesVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Ordenes.Include(o => o.Reserva);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Ordenes/Details/5
        [Authorize(Policy = "OrdenesDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }

        // GET: Ordenes/Create
        [Authorize(Policy = "OrdenesCrear")]
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Ordenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordene);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // GET: Ordenes/Edit/5
        [Authorize(Policy = "OrdenesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes.FindAsync(id);
            if (ordene == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // POST: Ordenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (id != ordene.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordene);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdeneExists(ordene.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // GET: Ordenes/Delete/5
        [Authorize(Policy = "OrdenesEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }

        // POST: Ordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordene = await _context.Ordenes.FindAsync(id);
            if (ordene != null)
            {
                _context.Ordenes.Remove(ordene);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdeneExists(int id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }
    }
}
