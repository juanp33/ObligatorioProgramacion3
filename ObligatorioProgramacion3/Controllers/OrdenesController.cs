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
    
    public class OrdenesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public OrdenesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // GET: Ordenes
        [Authorize(Policy = "OrdenesVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Ordenes.Include(o => o.Reserva);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Ordenes/Details/5
        [Authorize(Policy = "OrdenesDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }

        // GET: Ordenes/Create
        [Authorize(Policy = "OrdenesCrear")]
        public IActionResult Create()
        {
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id");
            return View();
        }

        // POST: Ordenes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ordene);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // GET: Ordenes/Edit/5
        [Authorize(Policy = "OrdenesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes.FindAsync(id);
            if (ordene == null)
            {
                return NotFound();
            }
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // POST: Ordenes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ReservaId,Total")] Ordene ordene)
        {
            if (id != ordene.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ordene);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrdeneExists(ordene.Id))
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
            ViewData["ReservaId"] = new SelectList(_context.Reservas, "Id", "Id", ordene.ReservaId);
            return View(ordene);
        }

        // GET: Ordenes/Delete/5
        [Authorize(Policy = "OrdenesEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ordene = await _context.Ordenes
                .Include(o => o.Reserva)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ordene == null)
            {
                return NotFound();
            }

            return View(ordene);
        }

        // POST: Ordenes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ordene = await _context.Ordenes.FindAsync(id);
            if (ordene != null)
            {
                _context.Ordenes.Remove(ordene);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrdeneExists(int id)
        {
            return _context.Ordenes.Any(e => e.Id == id);
        }
    }
}
