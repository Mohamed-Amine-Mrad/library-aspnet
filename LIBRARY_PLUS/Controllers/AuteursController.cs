using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LIBRARY_PLUS.Data;
using LIBRARY_PLUS.Models;
using Microsoft.AspNetCore.Authorization;

namespace LIBRARY_PLUS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuteursController : Controller
    {
        private readonly AppDbContext _context;

        public AuteursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Auteurs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Auteurs.ToListAsync());
        }

        // GET: Auteurs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs
                .FirstOrDefaultAsync(m => m.IdAuteur == id);
            if (auteur == null)
            {
                return NotFound();
            }

            return View(auteur);
        }

        // GET: Auteurs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Auteurs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAuteur,Nom,Prenom")] Auteur auteur)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auteur);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auteur);
        }

        // GET: Auteurs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur == null)
            {
                return NotFound();
            }
            return View(auteur);
        }

        // POST: Auteurs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAuteur,Nom,Prenom")] Auteur auteur)
        {
            if (id != auteur.IdAuteur)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auteur);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuteurExists(auteur.IdAuteur))
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
            return View(auteur);
        }

        // GET: Auteurs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auteur = await _context.Auteurs
                .FirstOrDefaultAsync(m => m.IdAuteur == id);
            if (auteur == null)
            {
                return NotFound();
            }

            return View(auteur);
        }

        // POST: Auteurs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var auteur = await _context.Auteurs.FindAsync(id);
            if (auteur != null)
            {
                _context.Auteurs.Remove(auteur);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuteurExists(int id)
        {
            return _context.Auteurs.Any(e => e.IdAuteur == id);
        }
    }
}
