using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;

namespace ObligatorioProgramacion3.Controllers
{
    public class ClientesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public ClientesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // GET: Clientes
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Clientes.Include(c => c.IdUsuariosNavigation);
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }

        // GET: Clientes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdUsuariosNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // GET: Clientes/Create
        public IActionResult Create()
        {
            ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Id");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,TipoCliente,IdUsuarios")] Cliente cliente)
        {
            try
            {
                ModelState.Remove("IdUsuariosNavigation");
                if (ModelState.IsValid)
                {
                    _context.Add(cliente);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Id", cliente.IdUsuarios);
                return View(cliente);
            }
           
             catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(cliente);
            }
        }

        // GET: Clientes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }
            ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Id", cliente.IdUsuarios);
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,TipoCliente,IdUsuarios")] Cliente cliente)
        {
            try
            {
                if (id != cliente.Id)
                {
                    return NotFound();
                }
                ModelState.Remove("IdUsuariosNavigation");
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(cliente);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ClienteExists(cliente.Id))
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

                ViewData["IdUsuarios"] = new SelectList(_context.Usuarios, "Id", "Id", cliente.IdUsuarios);
                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(cliente);
            }
        }

        // GET: Clientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes
                .Include(c => c.IdUsuariosNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }

        }

        private bool ClienteExists(int id)
        {
            return _context.Clientes.Any(e => e.Id == id);
        }
    }
}
