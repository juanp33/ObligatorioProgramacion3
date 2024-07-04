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
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace ObligatorioProgramacion3.Controllers
{
    
    public class ReservasController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public ReservasController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }
        [HttpPost]
        public  async Task<IActionResult> Cancelar(int reservaId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva == null)
            {
                return NotFound();
            }
            reserva.Estado = "Cancelada";

            
            _context.Reservas.Update(reserva);

          
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(MostrarReservas));
        } 
        [Authorize(Policy = "ReservasMostrarReservas")]
        public async Task<IActionResult> MostrarReservas()
        {
            var IdUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
            ViewBag.UserClaims = User.Claims;
            var reservas = await _context.Reservas
                .Include(r => r.IdRestauranteNavigation)
                .Where(r => r.UsuarioId == IdUsuario)
                .ToListAsync();

            return View(reservas);

        }
        [Authorize(Policy = "ReservasSeleccionarFecha")]
        [HttpPost]
        public IActionResult SeleccionarFecha(int restauranteId)
        {
          
            ViewData["RestauranteId"] = restauranteId;
            return View(restauranteId);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EnviarFecha(DateTime fecha, int restauranteId)
        {
           
            return RedirectToAction("SeleccionarMesa", new { fecha, restauranteId });
        }

        [Authorize(Policy = "ReservasSeleccionarMesa")]
        public async Task<IActionResult> SeleccionarMesa(DateTime fecha, int restauranteId)
        {
            
            var mesasOcupadas = await _context.Reservas.Where(reserva => reserva.FechaReserva == fecha && reserva.IdRestaurante == restauranteId).Select(reserva => reserva.MesaId).ToListAsync();

            var mesas = await _context.Mesas
         .Where(m => m.IdRestaurante == restauranteId)
         .ToListAsync();

            var viewModel = new SelectMesa
            {
                Fecha = fecha,
                restauranteId = restauranteId,

                ChechboxMesa = mesas.Select(m => new ChechboxMesa
                {
                    MesaId = m.Id,

                    EstaOcupada = mesasOcupadas.Contains(m.Id)
                }).ToList()
            }; 

            return View(viewModel);
        }

        [Authorize(Policy = "ReservasCrearReserva")]
        [Authorize(Policy = "ReservasCrearReserva")]
        public IActionResult CrearReserva(int MesaId, DateTime fecha, int restauranteId)
        {
            var clienteId = _context.Clientes
                .Where(r => r.IdUsuarios == int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)))
                .Select(r => r.Id)
                .FirstOrDefault();

            var mesa = _context.Mesas.FirstOrDefault(m => m.Id == MesaId);
            if (mesa == null)
            {
                return NotFound();
            }

            var reserva = new Reserva
            {
                MesaId = mesa.Id,
                FechaReserva = fecha,
                Estado = "Pendiente",
                ClienteId = clienteId,
                IdRestaurante = restauranteId,
                UsuarioId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            };

            ViewData["MesaId"] = mesa.Id;
            ViewData["NumeroMesa"] = mesa.NumeroMesa;
            ViewData["Fecha"] = fecha;
            ViewData["IdRestaurante"] = restauranteId;

            return View(reserva);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReserva([Bind("MesaId,FechaReserva,ClienteId,Estado,IdRestaurante,UsuarioId")] Reserva reserva)
        {
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

        [Authorize(Policy = "ReservasVer")] 
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Reservas.Include(r => r.Cliente).Include(r => r.Mesa);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Reservas/Details/5
        [Authorize(Policy = "ReservasDetalle")]
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
        [Authorize(Policy = "ReservasCrear")]
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
        [Authorize(Policy = "ReservasEditar")]
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
        [Authorize(Policy = "ReservasEliminar")]
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
