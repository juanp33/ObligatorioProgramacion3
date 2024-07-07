using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;
using Microsoft.AspNetCore.Authorization;

namespace ObligatorioProgramacion3.Controllers
{
    public class PermisoesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public PermisoesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        [Authorize(Policy = "PermisosRolesYPermisos")]
        public async Task<IActionResult> RolesYPermisos()
        {
          
            var roles = await _context.Roles.ToListAsync();
            var permisos = await _context.Permisos.ToListAsync();
            var rolPermisos = await _context.RolPermisos.ToListAsync();

            var modelo = new RolPermisoViewModel
            {
                Roles = roles,
                Permisos = permisos,
                RolPermisos = rolPermisos
            };

            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> ActualizarPermisos(List<int> RolIds)
        {
            foreach (var rolId in RolIds)
            {

                var permisoIds = Request.Form[$"PermisoIds_{rolId}"].Select(int.Parse).ToList();


                var rol = await _context.Roles.FindAsync(rolId);



                var permisosValidos = await _context.Permisos
                    .Where(p => permisoIds.Contains(p.PermisoId))
                    .ToListAsync();


                var permisosExistentes = _context.RolPermisos.Where(rp => rp.RolId == rolId);
                _context.RolPermisos.RemoveRange(permisosExistentes);


                foreach (var permiso in permisosValidos)
                {
                    _context.RolPermisos.Add(new RolPermiso
                    {
                        RolId = rolId,
                        PermisoId = permiso.PermisoId
                    });
                }


                await _context.SaveChangesAsync();


                var usuariosConRol = await _context.Usuarios
                    .Include(u => u.Rol)
                    .Where(u => u.RolId == rolId)
                    .ToListAsync();

                foreach (var usuario in usuariosConRol)
                {
                    var claimsActuales = (await HttpContext.AuthenticateAsync()).Principal.Claims.ToList();

                    var permisosARemover = claimsActuales.Where(c => c.Type == "Permission");

                    foreach (var claim in permisosARemover.ToList())
                    {
                        claimsActuales.Remove(claim);
                    }


                    foreach (var permiso in permisosValidos)
                    {
                        claimsActuales.Add(new Claim("Permission", permiso.Nombre));
                    }

                    var claimsIdentity = new ClaimsIdentity(claimsActuales, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
                }
            }

            
            return RedirectToAction(nameof(RolesYPermisos));
        }
        // GET: Permisoes

        [Authorize(Policy = "PermisosVer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Permisos.ToListAsync());
        }

        // GET: Permisoes/Details/5
        [Authorize(Policy = "PermisosDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .FirstOrDefaultAsync(m => m.PermisoId == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // GET: Permisoes/Create
        [Authorize(Policy = "PermisosCrear")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Permisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PermisoId,Nombre,Descripcion")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // GET: Permisoes/Edit/5
        [Authorize(Policy = "PermisosEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }
            return View(permiso);
        }

        // POST: Permisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PermisoId,Nombre,Descripcion")] Permiso permiso)
        {
            if (id != permiso.PermisoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermisoExists(permiso.PermisoId))
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
            return View(permiso);
        }

        // GET: Permisoes/Delete/5
        [Authorize(Policy = "PermisosEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var permiso = await _context.Permisos
                .FirstOrDefaultAsync(m => m.PermisoId == id);
            if (permiso == null)
            {
                return NotFound();
            }

            return View(permiso);
        }

        // POST: Permisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                _context.Permisos.Remove(permiso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PermisoExists(int id)
        {
            return _context.Permisos.Any(e => e.PermisoId == id);
        }
    }
}
