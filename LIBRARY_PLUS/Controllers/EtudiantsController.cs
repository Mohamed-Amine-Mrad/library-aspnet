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
    public class EtudiantsController : Controller
    {
        private readonly AppDbContext _context;

        public EtudiantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Etudiants
        public async Task<IActionResult> Index()
        {
            var etudiants = await _context.Etudiants
                .Include(e => e.Emprunts)
                .ToListAsync();

            foreach (var etudiant in etudiants)
            {
                etudiant.ScoreEmprunt = etudiant.Emprunts != null
                    ? etudiant.Emprunts.Count
                    : 0;
            }

            return View(etudiants);
        }

        // GET: Etudiants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etudiant = await _context.Etudiants
                .Include(e => e.Emprunts)
                .FirstOrDefaultAsync(m => m.IdEtudiant == id);

            if (etudiant != null)
            {
                etudiant.ScoreEmprunt = etudiant.Emprunts != null
                    ? etudiant.Emprunts.Count
                    : 0;
            }
            if (etudiant == null)
            {
                return NotFound();
            }

            return View(etudiant);
        }

        // GET: Etudiants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Etudiants/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEtudiant,Nom,Prenom,Email,Classe,ScoreEmprunt")] Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(etudiant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(etudiant);
        }

        // GET: Etudiants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etudiant = await _context.Etudiants
                .Include(e => e.Emprunts)
                .FirstOrDefaultAsync(e => e.IdEtudiant == id);

            if (etudiant != null)
            {
                etudiant.ScoreEmprunt = etudiant.Emprunts != null
                    ? etudiant.Emprunts.Count
                    : 0;
            }
            if (etudiant == null)
            {
                return NotFound();
            }
            return View(etudiant);
        }

        // POST: Etudiants/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEtudiant,Nom,Prenom,Email,Classe,ScoreEmprunt")] Etudiant etudiant)
        {
            if (id != etudiant.IdEtudiant)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(etudiant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EtudiantExists(etudiant.IdEtudiant))
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
            return View(etudiant);
        }

        // GET: Etudiants/Delete/5
        // GET: Etudiants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etudiant = await _context.Etudiants
                .Include(e => e.Emprunts)
                .FirstOrDefaultAsync(m => m.IdEtudiant == id);

            if (etudiant == null)
            {
                return NotFound();
            }

            etudiant.ScoreEmprunt = etudiant.Emprunts != null
                ? etudiant.Emprunts.Count
                : 0;

            return View(etudiant);
        }

        // POST: Etudiants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant != null)
            {
                _context.Etudiants.Remove(etudiant);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EtudiantExists(int id)
        {
            return _context.Etudiants.Any(e => e.IdEtudiant == id);
        }
    }
}
