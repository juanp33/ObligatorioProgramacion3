﻿using System;
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
            using (var client = new HttpClient())
            {
                try
                {
                    string moneda;
                    if (restauranteId == 1)
                    {
                        moneda = "UYU";
                    }
                    else if (restauranteId == 2)
                    {
                        moneda = "MXN";
                    }
                    else { moneda = "EUR"; }
                    string url = $"http://api.currencylayer.com/live?access_key=b6fb3ee2a8859b4237975e1d708cb64e&currencies={moneda}";
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
                        if (restauranteId == 1)
                        {
                            return data.quotes.USDUYU;
                        }
                        else if (restauranteId == 2)
                        {

                            return data.quotes.USDMXN;
                        }
                        else
                        {
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
            using (var client = new HttpClient())
            {
                try
                {
                    string lat;
                    string lon;

                    if (restauranteId == 1)
                    {
                        lon = "-56.1913095";
                        lat = "-34.9058916"; // Coordenadas para Montevideo, Uruguay
                    }
                    else if (restauranteId == 2)
                    {
                        lon = "-99.1331785";
                        lat = "19.4326296"; // Coordenadas para Ciudad de México, México
                    }
                    else
                    {
                        lon = "-3.7035825";
                        lat = "40.4167047"; // Coordenadas para Madrid, España
                    }

                    string apiKey = "13c5e50bbda5f27893d4e5fc19a4f058";
                    string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={apiKey}";

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
            
            var items = _carritoService.ObtenerCarritoItems();
            var total = _carritoService.ObtenerTotal();
            ViewData["SubTotal"] = total;
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

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

            Ordene orden = new Ordene
            {
                ReservaId = reservaId,
                Total = _carritoService.ObtenerTotal()
            };

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();
          
            foreach(var item in items)
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
            
            var ultimoClima = await _context.Climas
                                        .OrderByDescending(c => c.Fecha)
                                        .FirstOrDefaultAsync();
            var ultimaCotizacion = await _context.Cotizacions
                                        .OrderByDescending(c => c.Id)
                                        .FirstOrDefaultAsync();
            var pago = new Pago
            {
                ReservaId = reservaId,
                Monto = total,
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
