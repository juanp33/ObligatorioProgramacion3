using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;

namespace ObligatorioProgramacion3.Controllers
{
    
    public class ReseñaController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public ReseñaController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }
     
        // GET: Reseña
        [Authorize(Policy = "ReseñasVer")]
        public async Task<IActionResult> Index()
        {
            var obligatorioProgramacion3Context = _context.Reseñas.Include(o => o.Restaurante).Include(o => o.Cliente);    
            return View(await obligatorioProgramacion3Context.ToListAsync());
        }
        
        public async Task<IActionResult> Reseñas()
        {

            using (var context = new ObligatorioProgramacion3Context())
            {
                var reseñas = await context.Reseñas
                    .Include(r => r.Cliente)
                    .Include(r => r.Restaurante)
                    .ToListAsync();

                return View(reseñas);
            }
        }

        public IActionResult CrearReseñaUsuario()
        {
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre");
            return View();

        }
        [HttpPost]
      
        public async Task<IActionResult> CrearReseñaUsuario ([Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {

            if (ModelState.IsValid)
            {

                @reseña.FechaReseña = DateTime.Now;
                if (User.Identity.IsAuthenticated)
                {
                     var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim == null || !int.TryParse(claim.Value, out var usuarioId))
                    {
                        return View();
                    }
                    
                    var clienteID = await _context.Clientes.FirstOrDefaultAsync(cliente => cliente.IdUsuarios == usuarioId);
                    reseña.ClienteId = clienteID.Id;
                }
                else
                {
                   
                    reseña.ClienteId = 1015;
                }
                _context.Add(reseña);               
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Reseñas));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre", reseña.RestauranteId);
            return View(reseña);
        }


        // GET: Reseña/Details/5
        [Authorize(Policy = "ReseñasDetalle")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // GET: Reseña/Create
        [Authorize(Policy = "ReseñasCrear")]
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id");
            ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre");
            return View();
        }

        // POST: Reseña/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(reseña);
                    @reseña.FechaReseña = DateTime.Now;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
                ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Nombre", reseña.RestauranteId);
                return View(reseña);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(reseña);
            }

        }

        // GET: Reseña/Edit/5
        [Authorize(Policy = "ReseñasEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var reseña = await _context.Reseñas.FindAsync(id);
                if (reseña == null)
                {
                    return NotFound();
                }
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
                ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", reseña.RestauranteId);
                return View(reseña);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(id);
            }
        }

        // POST: Reseña/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClienteId,RestauranteId,Puntaje,Comentario,FechaReseña")] Reseña reseña)
        {
            try
            {
                if (id != reseña.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(reseña);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ReseñaExists(reseña.Id))
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
                ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Id", reseña.ClienteId);
                ViewData["RestauranteId"] = new SelectList(_context.Restaurantes, "Id", "Id", reseña.RestauranteId);
                return View(reseña);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(reseña);
            }

        }

        // GET: Reseña/Delete/5
        [Authorize(Policy = "ReseñasEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reseña = await _context.Reseñas
                .Include(r => r.Cliente)
                .Include(r => r.Restaurante)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reseña == null)
            {
                return NotFound();
            }

            return View(reseña);
        }

        // POST: Reseña/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var reseña = await _context.Reseñas.FindAsync(id);
                if (reseña != null)
                {
                    _context.Reseñas.Remove(reseña);
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

        private bool ReseñaExists(int id)
        {
            return _context.Reseñas.Any(e => e.Id == id);
        }
    }
}
