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
    
    public class MesasController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public MesasController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }
        
        // GET: Mesas
        [Authorize(Policy = "MesasVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Mesas.Include(m => m.IdRestauranteNavigation);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Mesas/Details/5
        [Authorize(Policy = "MesasDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mesa = await _context.Mesas
                .Include(m => m.IdRestauranteNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mesa == null)
            {
                return NotFound();
            }

            return View(mesa);
        }

        // GET: Mesas/Create
        [Authorize(Policy = "MesasCrear")]
        public IActionResult Create()
        {
            ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección");
            return View();
        }

        // POST: Mesas1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NumeroMesa,Capacidad,Estado,IdRestaurante")] Mesa mesa)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(mesa);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", mesa.IdRestaurante);
                return View(mesa);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(mesa);
            }

        }

        // GET: Mesas/Edit/5
        [Authorize(Policy = "MesasEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var mesa = await _context.Mesas.FindAsync(id);
                if (mesa == null)
                {
                    return NotFound();
                }
                ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", mesa.IdRestaurante);
                return View(mesa);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(id);
            }

        }

        // POST: Mesas1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NumeroMesa,Capacidad,Estado,IdRestaurante")] Mesa mesa)
        {
            try
            {
                if (id != mesa.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(mesa);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!MesaExists(mesa.Id))
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
                ViewData["IdRestaurante"] = new SelectList(_context.Restaurantes, "Id", "Dirección", mesa.IdRestaurante);
                return View(mesa);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(mesa);
            }
        }

        // GET: Mesas/Delete/5
        [Authorize(Policy = "MesasEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var mesa = await _context.Mesas
                    .Include(m => m.IdRestauranteNavigation)
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (mesa == null)
                {
                    return NotFound();
                }

                return View(mesa);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(id);
            }
        }

        // POST: Mesas1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var mesa = await _context.Mesas.FindAsync(id);
                if (mesa != null)
                {
                    _context.Mesas.Remove(mesa);
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

        private bool MesaExists(int id)
        {
            return _context.Mesas.Any(e => e.Id == id);
        }
    }
}

