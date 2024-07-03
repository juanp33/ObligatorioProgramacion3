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
    
    public class PlatoesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public PlatoesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }


        public async Task<IActionResult> MenuSoloVer(int restauranteId)
        {
            var Platos = await _context.Platos.Where(p => p.IdRestaurante == restauranteId).ToListAsync();
            return View(Platos);
        }
        // GET: Platoes
        [Authorize(Policy = "PlatoesVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Platos.Include(p => p.IdRestauranteNavigation);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Platoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plato = await _context.Platos
                .Include(p => p.IdRestauranteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plato == null)
            {
                return NotFound();
            }

            return View(plato);
        }
        
        
        [Authorize(Policy = "PlatoesCrear")]
        // GET: Platoes1/Create
        public IActionResult Create()
        {
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección");
            return View();
        }


        // POST: Platoes1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NombrePlato,Descripción,Precio,Imagen,IdRestaurante")] Plato plato)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", plato.IdRestaurante);
            return View(plato);
        }

        // GET: Platoes/Edit/5
        [Authorize(Policy = "PlatoesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plato = await _context.Platos.FindAsync(id);
            if (plato == null)
            {
                return NotFound();
            }
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", plato.IdRestaurante);
            return View(plato);
        }

        // POST: Platoes1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NombrePlato,Descripción,Precio,Imagen,IdRestaurante")] Plato plato)
        {
            if (id != plato.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatoExists(plato.Id))
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
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", plato.IdRestaurante);
            return View(plato);
        }

        // GET: Platoes/Delete/5
        [Authorize(Policy = "PlatoesEliminar")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plato = await _context.Platos
                .Include(p => p.IdRestauranteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plato == null)
            {
                return NotFound();
            }

            return View(plato);
        }

        // POST: Platoes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plato = await _context.Platos.FindAsync(id);
            if (plato != null)
            {
                _context.Platos.Remove(plato);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatoExists(int id)
        {
            return _context.Platos.Any(e => e.Id == id);
        }


        [Authorize(Policy = "PlatoesSeleccionarPlato")]
        [HttpPost]
        public async Task<IActionResult> SeleccionarPlato(int ReservaId) 
        {
            var restauranteId = _context.Reservas.Where(r => r.Id == ReservaId).Select(r => r.IdRestaurante).FirstOrDefault();
            var Platos = await _context.Platos.Where(p => p.IdRestaurante == restauranteId).ToListAsync();
            ViewData["ReservaId"] = ReservaId;
            return View(Platos);
            

        }
    }
}
