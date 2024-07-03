using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
            _carritoService = carritoService;
        }
        private async Task<decimal> ObtenerTipoDeCambio()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string url = "http://api.currencylayer.com/live?access_key=b6fb3ee2a8859b4237975e1d708cb64e&currencies=UYU";
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(content);
                        var tipoDeCambio = new Cotizacion
                        {
                            Cotizacion1 = data.quotes.USDUYU,
                            FechaCotizacion = DateTime.Now
                        };
                        _context.Cotizacions.Add(tipoDeCambio);
                        await _context.SaveChangesAsync();
                        return (data.quotes.USDUYU);
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    return 1;

                }

            }
        }

        //private async Task<string> ObtenerClima(int lat, int lon)
        //{

        //}
        // GET: Pagoes
        [Authorize(Policy = "PagosPagarReserva")]
        public async Task<IActionResult> PagarReserva(int reservaId)
        {

            var items = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var tipoCliente = _context.Clientes
                          .Where(c => c.IdUsuarios == userId)
                          .Select(c => c.TipoCliente)
                          .FirstOrDefault();
            decimal descuento = 1;

            if (tipoCliente == "FRECUENTE")
            {
                descuento = descuento - (0.10m);
            }
            else if (tipoCliente == "VIP")
            {
                descuento = descuento - (0.20m);
            }

            decimal tipoDeCambio = await ObtenerTipoDeCambio();
            total = total * descuento;
            ViewData["descuento"] = descuento;
            ViewData["TipoDeCambio"] = tipoDeCambio;
            ViewData["reservaId"] = reservaId;
            var carritoViewModel = new CarritoViewModel
            {
                Items = items,
                Total = total
            };
            return View(carritoViewModel);
        }
        [HttpPost]
        [Authorize(Policy = "PagosPagarReserva")]
        public async Task<IActionResult> ConfirmarPago(string MetodoPago, int reservaId, decimal descuento)
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
            var total = _carritoService.ObtenerTotal() * descuento;
            var orden = new Ordene
            {
                ReservaId = reservaId,
                Total = _carritoService.ObtenerTotal(),
            };
            _context.Add(orden);
            await _context.SaveChangesAsync();
            
            foreach (var item in items)
            {
                _context.OrdenDetalles.Add(new OrdenDetalle
                {
                    Cantidad = item.Cantidad,
                    PlatoId = item.PlatoId,
                    OrdenId = orden.Id
                });

            }
            await _context.SaveChangesAsync();
            var pago = new Pago
            {
                ReservaId = reservaId,
                Monto = total,
                FechaPago = DateTime.Now,
                MetodoPago = MetodoPago
            };
            var reservaActualizado = _context.Reservas.Find(reservaId);
            reservaActualizado.Estado = "Confirmada";
            _context.Reservas.Update(reservaActualizado);
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();


            _carritoService.LimpiarCarrito();

            return RedirectToAction("Index", "Home");
        }

        [Authorize(Policy = "PagosVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Pagos.Include(p => p.IdClimaNavigation).Include(p => p.IdCotizacionNavigation).Include(p => p.Reserva);
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
                .Include(p => p.IdClimaNavigation)
                .Include(p => p.IdCotizacionNavigation)
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
            ViewData["IdClima"] = new SelectList(_context.Climas, "Id", "Id");
            ViewData["IdCotizacion"] = new SelectList(_context.Cotizacions, "Id", "Id");
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Estado");
            return View();
        }

        // POST: Pagoes1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Monto,FechaPago,MetodoPago,IdClima,IdCotizacion")] Pago pago)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdClima"] = new SelectList(_context.Climas, "Id", "Id", pago.IdClima);
            ViewData["IdCotizacion"] = new SelectList(_context.Cotizacions, "Id", "Id", pago.IdCotizacion);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Estado", pago.ReservaId);
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
            ViewData["IdClima"] = new SelectList(_context.Climas, "Id", "Id", pago.IdClima);
            ViewData["IdCotizacion"] = new SelectList(_context.Cotizacions, "Id", "Id", pago.IdCotizacion);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Estado", pago.ReservaId);
            return View(pago);
        }

        // POST: Pagoes1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Monto,FechaPago,MetodoPago,IdClima,IdCotizacion")] Pago pago)
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
            ViewData["IdClima"] = new SelectList(_context.Climas, "Id", "Id", pago.IdClima);
            ViewData["IdCotizacion"] = new SelectList(_context.Cotizacions, "Id", "Id", pago.IdCotizacion);
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Estado", pago.ReservaId);
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
                .Include(p => p.IdClimaNavigation)
                .Include(p => p.IdCotizacionNavigation)
                .Include(p => p.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null)
            {
                return NotFound();
            }

            return View(pago);
        }

        // POST: Pagoes1/Delete/5
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
