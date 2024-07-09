using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ObligatorioProgramacion3.Models;

namespace ObligatorioProgramacion3.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public UsuariosController(ObligatorioProgramacion3Context context)
        {
            _context = context;


        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(Usuario model)
        {
            ModelState.Remove("Rol");
            ModelState.Remove("Email");
            ModelState.Remove("RolId");
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _context.Usuarios
                        .Include(u => u.Rol)
                        .ThenInclude(r => r.RolPermisos)
                        .ThenInclude(rp => rp.Permiso)
                        .FirstOrDefaultAsync(u => u.Nombre == model.Nombre && u.Contraseña == model.Contraseña);

                    if (user != null)
                    {
                        var claims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Nombre)
            };

                        foreach (var permiso in user.Rol.RolPermisos.Select(rp => rp.Permiso.Nombre))
                        {
                            claims.Add(new Claim("Permission", permiso));
                        }

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                        return RedirectToAction("Index", "Home");
                    }

                    ViewBag.ErrorMessage = "Nombre de usuario o contraseña incorrectos.";
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(model);
            }

        }



        // GET: Usuarios


        [Authorize(Policy = "UsuariosVer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Usuarios.ToListAsync());
        }

        [Authorize(Policy = "UsuariosDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuarios/Create

        public IActionResult RegistroUsuario()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistroUsuario(RegistroViewModel model)
        {

            ModelState.Remove("TipoCliente");

            ModelState.Remove("Rol");

            ModelState.Remove("EmailCliente");




            try
            {
                if (ModelState.IsValid)
                {
                    var usuario = new Usuario
                    {
                        Nombre = model.NombreUsuario,
                        Email = model.EmailUsuario,
                        Contraseña = model.Contraseña,
                        RolId = 2
                    };
                    if (await _context.Usuarios.AnyAsync(u => u.Nombre == usuario.Nombre))
                    {
                        TempData["ErrorMessage"] = "El nombre del usuario ya existe.";
                        return RedirectToAction("RegistroUsuario");
                    }

                    if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
                    {
                        TempData["ErrorMessage"] = "El email del usuario ya existe.";
                        return RedirectToAction("RegistroUsuario");
                    }

                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    var cliente = new Cliente
                    {
                        Nombre = model.NombreCliente,
                        Email = model.EmailUsuario,
                        TipoCliente = "Nuevo",
                        IdUsuarios = usuario.Id
                    };


                    usuario.Cliente = cliente;

                    _context.Add(cliente);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " ;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(model);
            }


        }


        // POST: Usuarios/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Policy = "UsuariosCrear")]
        
        public IActionResult Create()
        {
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Email,Contraseña,Rol")] Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(usuario);
            }

        }

        [Authorize(Policy = "UsuariosEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre", usuario.RolId);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Email,Contraseña,RolId")] Usuario usuario)
        {
            try
            {
                if (id != usuario.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(usuario);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!UsuarioExists(usuario.Id))
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
                ViewData["RolId"] = new SelectList(_context.Roles, "RolId", "Nombre", usuario.RolId);
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(usuario);
            }

        }

        // GET: Usuarios/Delete/5

        [Authorize(Policy = "UsuariosEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (usuario == null)
                {
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(id);
            }

        }

        // POST: Usuarios/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el usuario: " + ex.Message;
                return View(id);
            }
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
