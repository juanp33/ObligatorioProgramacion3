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
    public class RolesController : Controller
    {
        private readonly ObligatorioProgramacion3Context _context;

        public RolesController(ObligatorioProgramacion3Context context)
        {
            _context = context;
        }

        // GET: Roles
        [Authorize(Policy = "RolesVer")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Roles.ToListAsync());
        }

        // GET: Roles/Details/5
        [Authorize(Policy = "RolesDetalle")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RolId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // GET: Roles/Create
        [Authorize(Policy = "RolesCrear")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RolId,Nombre,Descripcion")] Role role)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(role);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(role);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(role);
            }

        }

        // GET: Roles/Edit/5
        [Authorize(Policy = "RolesEditar")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Roles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RolId,Nombre,Descripcion")] Role role)
        {
            try
            {
                if (id != role.RolId)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(role);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RoleExists(role.RolId))
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
                return View(role);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Hubo un problema al registrar el cambio: " + ex.Message;
                return View(role);
            }

        }

        // GET: Roles/Delete/5
        [Authorize(Policy = "RolesEliminar")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var role = await _context.Roles
                .FirstOrDefaultAsync(m => m.RolId == id);
            if (role == null)
            {
                return NotFound();
            }

            return View(role);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var role = await _context.Roles.FindAsync(id);
                if (role != null)
                {
                    _context.Roles.Remove(role);
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

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.RolId == id);
        }
    }
}
