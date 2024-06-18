using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;


namespace ObligatorioProgramacion3.Controllers
{
    public class PermisosController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public PermisosController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // Acción para listar roles y permisos
        public async Task<IActionResult> Index()
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

            // Redireccionar a alguna acción deseada después de actualizar los permisos
            return RedirectToAction(nameof(Index));
        }
    }
}
