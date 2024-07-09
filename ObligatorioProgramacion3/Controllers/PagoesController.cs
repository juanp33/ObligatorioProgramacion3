using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            _carritoService= carritoService;
        }
        private async Task<decimal> ObtenerTipoDeCambio(int reservaID)
        {
            var restauranteId = _context.Reservas.Where(r => r.Id == reservaID).Select(r => r.IdRestaurante).FirstOrDefault();
            var moneda =_context.Restaurantes.Where(r => r.Id==restauranteId).Select(r => r.Moneda).FirstOrDefault();
            using (var client = new HttpClient())
            {
                try
                {
                   
                    string url = $"http://api.currencylayer.com/live?access_key=b6fb3ee2a8859b4237975e1d708cb64e&currencies={moneda}";
                    HttpResponseMessage response = await client.GetAsync(url);


                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(content);
                      
                        if (restauranteId == 1)
                        {
                            var tipoDeCambio = new Cotizacion
                            {
                                Cotizacion1 = data.quotes.USDUYU,
                                FechaCotizacion = DateTime.Now
                            };
                            _context.Cotizacions.Add(tipoDeCambio);
                            await _context.SaveChangesAsync();
                            return data.quotes.USDUYU;
                        }
                        else if (restauranteId == 2)
                        {
                            var tipoDeCambio = new Cotizacion
                            {
                                Cotizacion1 = data.quotes.USDMXN,
                                FechaCotizacion = DateTime.Now
                            };
                            _context.Cotizacions.Add(tipoDeCambio);
                            await _context.SaveChangesAsync();
                            return data.quotes.USDMXN;
                        }
                        else
                        {
                            var tipoDeCambio = new Cotizacion
                            {
                                Cotizacion1 = data.quotes.USDEUR,
                                FechaCotizacion = DateTime.Now
                            };
                            _context.Cotizacions.Add(tipoDeCambio);
                            await _context.SaveChangesAsync();
                            return data.quotes.USDEUR;
                        }
                    }
                }
                catch (Exception ex)
                {


                }
                return 1;
            }
        }
        private async Task<bool> ObtenerClima(int reservaID)
        {
            var restauranteId = _context.Reservas.Where(r => r.Id == reservaID).Select(r => r.IdRestaurante).FirstOrDefault();
            var ciudad = _context.Restaurantes.Where(r => r.Id == restauranteId).Select(r => r.Dirección).FirstOrDefault();
            using (var client = new HttpClient())
            {
                try
                {
                   

                    string apiKey = "13c5e50bbda5f27893d4e5fc19a4f058";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?q={ciudad}&units=metric&appid={apiKey}";



                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        using JsonDocument doc = JsonDocument.Parse(content);
                        JsonElement weatherArray = doc.RootElement.GetProperty("weather");
                        JsonElement mainElement = doc.RootElement.GetProperty("main");

                        bool isRaining = false;
                        string descripcion = string.Empty;
                        foreach (JsonElement element in weatherArray.EnumerateArray())
                        {
                            string main = element.GetProperty("main").GetString();
                            descripcion = element.GetProperty("description").GetString();

                            if (main == "Rain")
                            {
                                isRaining = true;
                                break;
                            }
                        }

                        decimal temperatura = mainElement.GetProperty("temp").GetDecimal();

                        Clima clima = new Clima
                        {
                            DescripciónClima = descripcion,
                            Temperatura = temperatura,
                            Fecha = DateOnly.FromDateTime(DateTime.Now),
                        };
                        _context.Add(clima);
                        await _context.SaveChangesAsync();

                        return isRaining;
                    }
                }
                catch (Exception ex)
                {

                }

                return false;
            }
        }

        // GET: Pagoes
        [Authorize(Policy = "PagosPagarReserva")]
        public async Task<IActionResult> PagarReserva(int reservaId)
        {
            var ordenId = await _context.Ordenes.Where(x => x.ReservaId == reservaId).Select(x=>x.Id).FirstOrDefaultAsync();
          
            List<CarritoItem> items = new List<CarritoItem>();
            var orden = await _context.Ordenes.Where(orden => orden.Id == ordenId).Select(orden => orden.Id).FirstOrDefaultAsync();
            var listaDePlatos = await _context.OrdenDetalles.Where(orden => orden.OrdenId == ordenId).ToListAsync();
            decimal total = 0;
            foreach (var plato in listaDePlatos)
            {
                var nombrePlato = await _context.Platos.Where(c => c.Id == plato.PlatoId).Select(x => x.NombrePlato).FirstOrDefaultAsync();
                var descripcion = await _context.Platos.Where(c => c.Id == plato.PlatoId).Select(x => x.Descripción).FirstOrDefaultAsync();
                var precio = await _context.Platos.Where(c => c.Id == plato.PlatoId).Select(x => x.Precio).FirstOrDefaultAsync();
                CarritoItem carritoItem = new CarritoItem
                {
                    PlatoId = plato.PlatoId.Value,
                    NombrePlato = nombrePlato,
                    Cantidad = plato.Cantidad,
                    Precio = precio,
                    Descripción = descripcion
                };
                total = total + (precio * plato.Cantidad);
                items.Add(carritoItem);
            }

          
            ViewData["SubTotal"] = total;
            var userId = await _context.Reservas.Where(x => x.Id == reservaId).Select(x => x.UsuarioId).FirstOrDefaultAsync(); ;

            var tipoCliente = _context.Clientes
                          .Where(c => c.IdUsuarios == userId)
                          .Select(c => c.TipoCliente)
                          .FirstOrDefault();
            decimal descuento = 1;

            if (tipoCliente == "FRECUENTE" )
            {
                descuento = descuento - (0.10m);
            } else if (tipoCliente == "VIP")
            {
                descuento = descuento - (0.20m);
            }
            bool EstaLloviendo = await ObtenerClima(reservaId);
            var ultimoClima = await _context.Climas
                                        .OrderByDescending(c => c.Fecha)
                                        .FirstOrDefaultAsync();
            var temperatura = ultimoClima.Temperatura;
            if(temperatura < 10 && temperatura > 0)
            {
                descuento = descuento - (0.05m);
            }
            else if(temperatura <= 0)
            {
                descuento = descuento - (0.10m);
            }
            if (EstaLloviendo)
            {
                descuento = descuento - (0.05m);
            } 
            decimal tipoDeCambio = await ObtenerTipoDeCambio(reservaId);
            total = total * descuento;
            var restauranteId = _context.Reservas.Where(r => r.Id == reservaId).Select(r => r.IdRestaurante).FirstOrDefault();
            ViewData["TipoCliente"] = tipoCliente;
            ViewData["restauranteId"]=restauranteId;
            ViewData["descuento"] = descuento;
            ViewData["TipoDeCambio"] = tipoDeCambio;
            ViewData["reservaId"] = reservaId;
            ViewData["temperatura"] = temperatura;
            ViewData["estaLloviendo"] = EstaLloviendo;

            var carritoViewModel = new CarritoViewModel
            {
                Items = items,
                Total = total
            };
            return View(carritoViewModel);
        }
        [HttpPost]
        [Authorize(Policy = "PagosPagarReserva")]
        public async Task<IActionResult> ConfirmarPago(string MetodoPago, int reservaId, decimal descuento,decimal Total)
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
            
            
            var ultimoClima = await _context.Climas
                                        .OrderByDescending(c => c.Fecha)
                                        .FirstOrDefaultAsync();
            var ultimaCotizacion = await _context.Cotizacions
                                        .OrderByDescending(c => c.Id)
                                        .FirstOrDefaultAsync();
            var pago = new Pago
            {
                ReservaId = reservaId,
                Monto = Total,
                FechaPago = DateTime.Now,
                MetodoPago = MetodoPago,
                IdCotizacion=ultimaCotizacion.Id,
                IdClima=ultimoClima.Id

            };
            var reservaActualizado = _context.Reservas.Find(reservaId);
            reservaActualizado.Estado = "Confirmada";
            _context.Reservas.Update(reservaActualizado);
            _context.Pagos.Add(pago);
            await _context.SaveChangesAsync();

            
            _carritoService.LimpiarCarrito();

            return RedirectToAction("MostrarOrdenes","Ordenes");
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
            try
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(pago);
            }

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
            try
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(pago);
            }

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
            try
            {
                var pago = await _context.Pagos.FindAsync(id);
                if (pago != null)
                {
                    _context.Pagos.Remove(pago);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(id);
            }
        }

        private bool PagoExists(int id)
        {
            return _context.Pagos.Any(e => e.Id == id);
        }
    }
}
