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
    public class EmpruntsController : Controller
    {
        private readonly AppDbContext _context;

        public EmpruntsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Emprunts
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Emprunts
                .Include(e => e.Etudiant)
                .Include(e => e.Livre)
                .Include(e => e.Reservation);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Emprunts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Etudiant)
                .Include(e => e.Livre)
                .Include(e => e.Reservation)
                .FirstOrDefaultAsync(m => m.IdEmprunt == id);

            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // GET: Emprunts/Create
        public async Task<IActionResult> Create()
        {
            var emprunt = new Emprunt
            {
                DateEmprunt = DateTime.Today,
                DateRemisePrevue = DateTime.Today.AddDays(7)
            };

            await ChargerListesDeroulantes();
            return View(emprunt);
        }

        // POST: Emprunts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdEmprunt,DateEmprunt,IdLivre,IdEtudiant,IdReservation")] Emprunt emprunt)
        {
            var aujourdHui = DateTime.Today;

            emprunt.DateRemisePrevue = emprunt.DateEmprunt.AddDays(7);
            emprunt.DateRemiseEffective = null;

            var livre = await _context.Livres.FindAsync(emprunt.IdLivre);

            if (livre == null)
            {
                ModelState.AddModelError("IdLivre", "Le livre sélectionné est introuvable.");
            }
            else if (!livre.Disponible)
            {
                ModelState.AddModelError("IdLivre", "Ce livre est actuellement indisponible.");
            }

            if (emprunt.DateEmprunt.Date < aujourdHui)
            {
                ModelState.AddModelError("DateEmprunt", "La date d'emprunt doit être supérieure ou égale à la date du jour.");
            }

            if (emprunt.IdReservation.HasValue)
            {
                var reservation = await _context.Reservations.FindAsync(emprunt.IdReservation.Value);

                if (reservation == null)
                {
                    ModelState.AddModelError("IdReservation", "La réservation sélectionnée est introuvable.");
                }
                else
                {
                    if (emprunt.DateEmprunt.Date < reservation.DateReservation.Date)
                    {
                        ModelState.AddModelError("DateEmprunt", "La date d'emprunt doit être supérieure ou égale à la date de réservation.");
                    }

                    if (reservation.IdLivre != emprunt.IdLivre)
                    {
                        ModelState.AddModelError("IdReservation", "La réservation choisie ne correspond pas au livre sélectionné.");
                    }

                    if (reservation.IdEtudiant != emprunt.IdEtudiant)
                    {
                        ModelState.AddModelError("IdReservation", "La réservation choisie ne correspond pas à l'étudiant sélectionné.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(emprunt.IdLivre, emprunt.IdEtudiant, emprunt.IdReservation);
                return View(emprunt);
            }

            emprunt.EstEnRetard = false;
            emprunt.AlerteEnvoyee = false;

            _context.Add(emprunt);

            if (livre != null)
            {
                livre.Disponible = false;
                _context.Update(livre);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "L'emprunt a été créé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Emprunts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt == null)
            {
                return NotFound();
            }

            await ChargerListesDeroulantes(emprunt.IdLivre, emprunt.IdEtudiant, emprunt.IdReservation);
            return View(emprunt);
        }

        // POST: Emprunts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdEmprunt,DateEmprunt,DateRemisePrevue,DateRemiseEffective,EstEnRetard,AlerteEnvoyee,IdLivre,IdEtudiant,IdReservation")] Emprunt emprunt)
        {
            if (id != emprunt.IdEmprunt)
            {
                return NotFound();
            }

            var aujourdHui = DateTime.Today;

            var empruntExistant = await _context.Emprunts
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.IdEmprunt == emprunt.IdEmprunt);

            if (empruntExistant == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FindAsync(emprunt.IdLivre);

            if (livre == null)
            {
                ModelState.AddModelError("IdLivre", "Le livre sélectionné est introuvable.");
            }
            else if (!livre.Disponible && empruntExistant.IdLivre != emprunt.IdLivre)
            {
                ModelState.AddModelError("IdLivre", "Ce livre est actuellement indisponible.");
            }

            if (emprunt.DateEmprunt.Date < aujourdHui)
            {
                ModelState.AddModelError("DateEmprunt", "La date d'emprunt doit être supérieure ou égale à la date du jour.");
            }

            if (emprunt.DateRemisePrevue.Date <= emprunt.DateEmprunt.Date)
            {
                ModelState.AddModelError("DateRemisePrevue", "La date prévue de remise doit être strictement supérieure à la date d'emprunt.");
            }

            if (emprunt.DateRemiseEffective.HasValue &&
                emprunt.DateRemiseEffective.Value.Date < emprunt.DateEmprunt.Date)
            {
                ModelState.AddModelError("DateRemiseEffective", "La date de remise effective doit être supérieure ou égale à la date d'emprunt.");
            }

            if (emprunt.IdReservation.HasValue)
            {
                var reservation = await _context.Reservations.FindAsync(emprunt.IdReservation.Value);

                if (reservation == null)
                {
                    ModelState.AddModelError("IdReservation", "La réservation sélectionnée est introuvable.");
                }
                else
                {
                    if (emprunt.DateEmprunt.Date < reservation.DateReservation.Date)
                    {
                        ModelState.AddModelError("DateEmprunt", "La date d'emprunt doit être supérieure ou égale à la date de réservation.");
                    }

                    if (reservation.IdLivre != emprunt.IdLivre)
                    {
                        ModelState.AddModelError("IdReservation", "La réservation choisie ne correspond pas au livre sélectionné.");
                    }

                    if (reservation.IdEtudiant != emprunt.IdEtudiant)
                    {
                        ModelState.AddModelError("IdReservation", "La réservation choisie ne correspond pas à l'étudiant sélectionné.");
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(emprunt.IdLivre, emprunt.IdEtudiant, emprunt.IdReservation);
                return View(emprunt);
            }

            emprunt.EstEnRetard = emprunt.DateRemiseEffective.HasValue &&
                                  emprunt.DateRemiseEffective.Value.Date > emprunt.DateRemisePrevue.Date;

            try
            {
                if (empruntExistant.IdLivre != emprunt.IdLivre)
                {
                    var ancienLivre = await _context.Livres.FindAsync(empruntExistant.IdLivre);
                    if (ancienLivre != null)
                    {
                        ancienLivre.Disponible = true;
                        _context.Update(ancienLivre);
                    }

                    if (livre != null)
                    {
                        livre.Disponible = false;
                        _context.Update(livre);
                    }
                }
                else if (livre != null)
                {
                    livre.Disponible = emprunt.DateRemiseEffective.HasValue;
                    _context.Update(livre);
                }

                _context.Update(emprunt);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpruntExists(emprunt.IdEmprunt))
                {
                    return NotFound();
                }

                throw;
            }

            TempData["SuccessMessage"] = "L'emprunt a été modifié avec succès.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Emprunts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emprunt = await _context.Emprunts
                .Include(e => e.Etudiant)
                .Include(e => e.Livre)
                .Include(e => e.Reservation)
                .FirstOrDefaultAsync(m => m.IdEmprunt == id);

            if (emprunt == null)
            {
                return NotFound();
            }

            return View(emprunt);
        }

        // POST: Emprunts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emprunt = await _context.Emprunts.FindAsync(id);
            if (emprunt != null)
            {
                var livre = await _context.Livres.FindAsync(emprunt.IdLivre);
                if (livre != null)
                {
                    livre.Disponible = true;
                    _context.Update(livre);
                }

                _context.Emprunts.Remove(emprunt);
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "L'emprunt a été supprimé avec succès.";
            return RedirectToAction(nameof(Index));
        }

        private async Task ChargerListesDeroulantes(object? selectedLivre = null, object? selectedEtudiant = null, object? selectedReservation = null)
        {
            var livresDisponibles = await _context.Livres
                .Where(l => l.Disponible || (selectedLivre != null && l.IdLivre.ToString() == selectedLivre.ToString()))
                .ToListAsync();

            ViewData["IdLivre"] = new SelectList(
                livresDisponibles,
                "IdLivre",
                "Titre",
                selectedLivre
            );

            ViewData["IdEtudiant"] = new SelectList(
                await _context.Etudiants.ToListAsync(),
                "IdEtudiant",
                "NomComplet",
                selectedEtudiant
            );

            ViewData["IdReservation"] = new SelectList(
                await _context.Reservations.ToListAsync(),
                "IdReservation",
                "IdReservation",
                selectedReservation
            );
        }

        private bool EmpruntExists(int id)
        {
            return _context.Emprunts.Any(e => e.IdEmprunt == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Retourner(int id)
        {
            var emprunt = await _context.Emprunts
                .Include(e => e.Livre)
                .FirstOrDefaultAsync(e => e.IdEmprunt == id);

            if (emprunt == null)
            {
                return NotFound();
            }

            // Si déjà retourné → on ne fait rien
            if (emprunt.DateRemiseEffective.HasValue)
            {
                TempData["ErrorMessage"] = "Cet emprunt est déjà retourné.";
                return RedirectToAction(nameof(Index));
            }

            // Marquer comme retourné
            emprunt.DateRemiseEffective = DateTime.Today;

            // Vérifier retard
            emprunt.EstEnRetard = emprunt.DateRemiseEffective.Value.Date > emprunt.DateRemisePrevue.Date;

            // Rendre le livre disponible
            if (emprunt.Livre != null)
            {
                emprunt.Livre.Disponible = true;
                _context.Update(emprunt.Livre);
            }

            _context.Update(emprunt);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Livre retourné avec succès.";

            return RedirectToAction(nameof(Index));
        }
    }
}