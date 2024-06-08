using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;
using Microsoft.AspNetCore.Authorization;

using System.Linq;
using System.Threading.Tasks;

namespace ObligatorioProgramacion3.Controllers
{
    [Authorize(Policy = "ReservasVer")]
    public class ReservasController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public ReservasController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }
        public IActionResult SeleccionarFecha(int restauranteId)
        {
          
            ViewData["RestauranteId"] = restauranteId;
            return View(restauranteId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SeleccionarFecha(DateTime fecha, int restauranteId)
        {
           
            return RedirectToAction("SeleccionarMesa", new { fecha, restauranteId });
        }
        
       
        public IActionResult SeleccionarMesa(DateTime fecha, int restauranteId)
        {
            var mesasOcupadas = _context.Reservas.Where(reserva => reserva.FechaReserva == fecha).Select(reserva => reserva.MesaId).ToList();

            var mesas = _context.Mesas.ToList();

            var viewModel = new SelectMesa
            {
                Fecha = fecha,
                restauranteId= restauranteId,
                
                ChechboxMesa = mesas.Select(m => new ChechboxMesa
                {
                    MesaId = m.Id,
                    
                    EstaOcupada = mesasOcupadas.Contains(m.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        public IActionResult CrearReserva(int MesaId, DateTime fecha, int restauranteId)
        {
            var mesa = _context.Mesas.FirstOrDefault(m => m.Id == MesaId);
            if (mesa == null)
            {
                return NotFound();
            }

            var reserva = new Reserva
            {
                MesaId = mesa.Id,
                FechaReserva = fecha,
                Estado = "Confirmada",
                ClienteId = 1,
                IdRestaurante= restauranteId

            };
           
            ViewData["MesaId"] = mesa.Id;
            ViewData["NumeroMesa"] = mesa.NumeroMesa;
            ViewData["Fecha"] = fecha;
            ViewData["IdRestaurante"]= restauranteId;
            return View(reserva);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReserva([Bind("MesaId,FechaReserva,ClienteId,Estado,IdRestaurante")] Reserva reserva)
        {
            reserva.Estado = "Confirmada";
            if (ModelState.IsValid)
            {
                

                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MesaId"] = reserva.Id;
            ViewData["Fecha"] = reserva.FechaReserva;
            return View(reserva);
        }
        // GET: Reservas
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Reservas.Include(r => r.Cliente).Include(r => r.Mesa);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Reservas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // GET: Reservas/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Id");
            return View();
        }

        // POST: Reservas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reserva);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Id", reserva.MesaId);
            return View(reserva);
        }

        // GET: Reservas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Id", reserva.MesaId);
            return View(reserva);
        }

        // POST: Reservas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,MesaId,FechaReserva,Estado")] Reserva reserva)
        {
            if (id != reserva.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reserva);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservaExists(reserva.Id))
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
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reserva.ClienteId);
            ViewData["MesaId"] = new SelectList(_context.Mesas, "Id", "Id", reserva.MesaId);
            return View(reserva);
        }

        // GET: Reservas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reserva = await _context.Reservas
                .Include(r => r.Cliente)
                .Include(r => r.Mesa)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reserva == null)
            {
                return NotFound();
            }

            return View(reserva);
        }

        // POST: Reservas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reserva = await _context.Reservas.FindAsync(id);
            if (reserva != null)
            {
                _context.Reservas.Remove(reserva);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservaExists(int id)
        {
            return _context.Reservas.Any(e => e.Id == id);
        }
    }
}
