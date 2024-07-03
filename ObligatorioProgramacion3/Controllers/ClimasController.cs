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

    public class ClimasController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public ClimasController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // GET: Climas
        [Authorize(Policy = "ClimasVer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Climas.ToListAsync());
        }

        // GET: Climas/Details/5
        [Authorize(Policy = "ClimasDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clima == null)
            {
                return NotFound();
            }

            return View(clima);
        }

        // GET: Climas/Create
        [Authorize(Policy = "ClimasCrear")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Climas1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fecha,Temperatura,DescripciónClima")] Clima clima)
        {
            if (ModelState.IsValid)
            {
                _context.Add(clima);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clima);

        }

        // GET: Climas/Edit/5
        [Authorize(Policy = "ClimasEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas.FindAsync(id);
            if (clima == null)
            {
                return NotFound();
            }
            return View(clima);
        }

        // POST: Climas1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fecha,Temperatura,DescripciónClima")] Clima clima)
        {
            if (id != clima.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clima);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClimaExists(clima.Id))
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
            return View(clima);
        }

        // GET: Climas/Delete/5
        [Authorize(Policy = "ClimasEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var clima = await _context.Climas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clima == null)
            {
                return NotFound();
            }

            return View(clima);
        }

        // POST: Climas1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var clima = await _context.Climas.FindAsync(id);
            if (clima != null)
            {
                _context.Climas.Remove(clima);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClimaExists(int id)
        {
            return _context.Climas.Any(e => e.Id == id);
        }
    }
}
