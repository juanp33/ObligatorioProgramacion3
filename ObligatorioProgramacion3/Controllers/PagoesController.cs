using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;

namespace ObligatorioProgramacion3.Controllers
{
    
    public class PagoesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;
        private readonly CarritoService _carritoService;

        public PagoesController(ObligatorioProgramacion3Context context, CarritoService carritoService)
        {
            _context = context;
            _carritoService= carritoService;
        }

        // GET: Pagoes
        [Authorize(Policy = "PagosPagarReserva")]
        public IActionResult PagarReserva()
        {
            var items = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();
            var carritoViewModel = new CarritoViewModel
            {
                Items = items,
                Total = total
            };
            return View(carritoViewModel);
        }
        [HttpPost]
        [Authorize(Policy = "PagosPagarReserva")]
        public async Task<IActionResult> ConfirmarPago(string MetodoPago)
        {
            if (string.IsNullOrEmpty(MetodoPago) || MetodoPago.Length > 50)
            {
                ModelState.AddModelError("MetodoPago", "El método de pago es obligatorio y no puede exceder los 50 caracteres.");
                return View("PagarReserva", new CarritoViewModel
                {
                    Items = _carritoService.ObtenerCarritoItems(),
                    Total = _carritoService.ObtenerTotal()
                });
            }

            var items = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();

            var pago = new Pago
            {
                ReservaId = 0,
                Monto = total,
                FechaPago = DateTime.Now,
                MetodoPago = MetodoPago
            };

            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            
            _carritoService.LimpiarCarrito();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "PagosVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Pagos.Include(p => p.Reserva);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Pagoes/Details/5
        [Authorize(Policy = "PagosDetalle")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // GET: Pagoes/Create
        [Authorize(Policy = "PagosCrear")]
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Pagoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Monto,FechaPago,MetodoPago")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }

        // GET: Pagoes/Edit/5
        [Authorize(Policy = "PagosEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }

        // POST: Pagoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Monto,FechaPago,MetodoPago")] Pago pago)
        {
            if (id != pago.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PagoExists(pago.Id))
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
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", pago.ReservaId);
            return View(pago);
        }

        // GET: Pagoes/Delete/5
        [Authorize(Policy = "PagosEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pago = await _context.Pagos
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                _context.Pagos.Remove(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}
