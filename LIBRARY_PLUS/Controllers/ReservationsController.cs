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
    public class ReservationsController : Controller
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Reservations
                .Include(r => r.Etudiant)
                .Include(r => r.Livre);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Etudiant)
                .Include(r => r.Livre)
                .FirstOrDefaultAsync(m => m.IdReservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        public async Task<IActionResult> Create()
        {
            await ChargerListesDeroulantes();
            return View();
        }

        // POST: Reservations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReservation,DateReservation,Statut,IdLivre,IdEtudiant")] Reservation reservation)
        {
            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(reservation.IdLivre, reservation.IdEtudiant);
                return View(reservation);
            }

            _context.Add(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            await ChargerListesDeroulantes(reservation.IdLivre, reservation.IdEtudiant);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReservation,DateReservation,Statut,IdLivre,IdEtudiant")] Reservation reservation)
        {
            if (id != reservation.IdReservation)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(reservation.IdLivre, reservation.IdEtudiant);
                return View(reservation);
            }

            try
            {
                _context.Update(reservation);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservation.IdReservation))
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

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Etudiant)
                .Include(r => r.Livre)
                .FirstOrDefaultAsync(m => m.IdReservation == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task ChargerListesDeroulantes(object? selectedLivre = null, object? selectedEtudiant = null)
        {
            ViewData["IdLivre"] = new SelectList(
                await _context.Livres.ToListAsync(),
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
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.IdReservation == id);
        }
    }
}