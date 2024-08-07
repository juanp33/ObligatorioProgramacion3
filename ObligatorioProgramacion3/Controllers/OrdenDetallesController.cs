﻿using System;
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
    
    public class OrdenDetallesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public OrdenDetallesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // GET: OrdenDetalles
        [Authorize(Policy = "OrdenDetallesVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.OrdenDetalles.Include(o => o.Orden).Include(o => o.Plato);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: OrdenDetalles/Details/5
        [Authorize(Policy = "OrdenDetallesDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenDetalle = await _context.OrdenDetalles
                .Include(o => o.Orden)
                .Include(o => o.Plato)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordenDetalle == null)
            {
                return NotFound();
            }

            return View(ordenDetalle);
        }

        // GET: OrdenDetalles/Create
        [Authorize(Policy = "OrdenDetallesCrear")]
        public IActionResult Create()
        {
            try
            {
                ViewData["OrdenId"] = new SelectList(_context.Ordenes, "Id", "Id");
                ViewData["PlatoId"] = new SelectList(_context.Platos, "Id", "Id");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View();
            }
        }

        // POST: OrdenDetalles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrdenId,PlatoId,Cantidad")] OrdenDetalle ordenDetalle)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(ordenDetalle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["OrdenId"] = new SelectList(_context.Ordenes, "Id", "Id", ordenDetalle.OrdenId);
                ViewData["PlatoId"] = new SelectList(_context.Platos, "Id", "Id", ordenDetalle.PlatoId);
                return View(ordenDetalle);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(ordenDetalle);
            }
        }

        // GET: OrdenDetalles/Edit/5
        [Authorize(Policy = "OrdenDetallesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenDetalle = await _context.OrdenDetalles.FindAsync(id);
            if (ordenDetalle == null)
            {
                return NotFound();
            }
            ViewData["OrdenId"] = new SelectList(_context.Ordenes, "Id", "Id", ordenDetalle.OrdenId);
            ViewData["PlatoId"] = new SelectList(_context.Platos, "Id", "Id", ordenDetalle.PlatoId);
            return View(ordenDetalle);
        }

        // POST: OrdenDetalles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrdenId,PlatoId,Cantidad")] OrdenDetalle ordenDetalle)
        {
            try
            {
                if (id != ordenDetalle.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ordenDetalle);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrdenDetalleExists(ordenDetalle.Id))
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
                ViewData["OrdenId"] = new SelectList(_context.Ordenes, "Id", "Id", ordenDetalle.OrdenId);
                ViewData["PlatoId"] = new SelectList(_context.Platos, "Id", "Id", ordenDetalle.PlatoId);
                return View(ordenDetalle);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(ordenDetalle);
            }
        }

        // GET: OrdenDetalles/Delete/5
        [Authorize(Policy = "OrdenDetallesEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordenDetalle = await _context.OrdenDetalles
                .Include(o => o.Orden)
                .Include(o => o.Plato)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordenDetalle == null)
            {
                return NotFound();
            }

            return View(ordenDetalle);
        }

        // POST: OrdenDetalles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var ordenDetalle = await _context.OrdenDetalles.FindAsync(id);
                if (ordenDetalle != null)
                {
                    _context.OrdenDetalles.Remove(ordenDetalle);
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

        private bool OrdenDetalleExists(int id)
        {
            return _context.OrdenDetalles.Any(e => e.Id == id);
        }
    }
}
