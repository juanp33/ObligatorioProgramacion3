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
    
    public class RestaurantesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public RestaurantesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> SeleccionarRestauranteMenu()
        {
            return View(await _context.Restaurantes.ToListAsync());
        }
        // GET: Restaurantes
        [Authorize(Policy = "RestaurantesSeleccionarRestaurante")]
        public async Task<IActionResult> SeleccionarRestaurante()
        {
            return View(await _context.Restaurantes.ToListAsync());
        }

        // GET: Restaurantes
        [Authorize(Policy = "RestaurantesVer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Restaurantes.ToListAsync());
        }

        // GET: Restaurantes/Details/5
        [Authorize(Policy = "RestaurantesDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurante == null)
            {
                return NotFound();
            }

            return View(restaurante);
        }

        // GET: Restaurantes/Create
        [Authorize(Policy = "RestaurantesCrear")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Restaurantes1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Dirección,Teléfono,Descripcion,Imagen,Moneda")] Restaurante restaurante)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(restaurante);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(restaurante);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(restaurante);
            }
        }

        // GET: Restaurantes/Edit/5
        [Authorize(Policy = "RestaurantesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurantes.FindAsync(id);
            if (restaurante == null)
            {
                return NotFound();
            }
            return View(restaurante);
        }

        // POST: Restaurantes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Dirección,Teléfono,Descripcion,Imagen,Moneda")] Restaurante restaurante)
        {
            try
            {
                if (id != restaurante.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(restaurante);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RestauranteExists(restaurante.Id))
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
                return View(restaurante);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(restaurante);
            }
        }

        // GET: Restaurantes/Delete/5
        [Authorize(Policy = "RestaurantesEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var restaurante = await _context.Restaurantes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (restaurante == null)
            {
                return NotFound();
            }

            return View(restaurante);
        }

        // POST: Restaurantes1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var restaurante = await _context.Restaurantes.FindAsync(id);
                if (restaurante != null)
                {
                    _context.Restaurantes.Remove(restaurante);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private bool RestauranteExists(int id)
        {
            return _context.Restaurantes.Any(e => e.Id == id);
        }
    }
}
