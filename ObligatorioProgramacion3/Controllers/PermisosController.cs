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

        // Acción para actualizar permisos
        [HttpPost]
        public async Task<IActionResult> ActualizarPermisos(List<int> RolIds)
        {
            foreach (var rolId in RolIds)
            {
                // Obtener los permisos seleccionados para el rol
                var permisoIds = Request.Form[$"PermisoIds_{rolId}"].Select(int.Parse).ToList();

                // Verificar si el rol existe
                var rol = await _context.Roles.FindAsync(rolId);
                if (rol == null)
                {
                    continue; // Si el rol no existe, pasa al siguiente
                }

                // Verificar si los permisos existen
                var permisosValidos = await _context.Permisos
                    .Where(p => permisoIds.Contains(p.PermisoId))
                    .Select(p => p.PermisoId)
                    .ToListAsync();

                // Eliminar los permisos existentes para el rol
                var permisosExistentes = _context.RolPermisos.Where(rp => rp.RolId == rolId);
                _context.RolPermisos.RemoveRange(permisosExistentes);

                // Agregar los nuevos permisos
                foreach (var permisoId in permisosValidos)
                {
                    _context.RolPermisos.Add(new RolPermiso
                    {
                        RolId = rolId,
                        PermisoId = permisoId
                    });
                }
            }

            // Guardar los cambios
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
