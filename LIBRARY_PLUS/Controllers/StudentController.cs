using LIBRARY_PLUS.Data;
using LIBRARY_PLUS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LIBRARY_PLUS.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly AppDbContext _context;

        public StudentController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.Identity?.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login", "Account");
            }

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound("Aucun étudiant trouvé avec cet email.");
            }

            var today = DateTime.Today;
            var jPlus2 = today.AddDays(2);

            var emprunts = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.IdEtudiant == etudiant.IdEtudiant)
                .ToListAsync();

            var reservations = await _context.Reservations
                .Include(r => r.Livre)
                .Where(r => r.IdEtudiant == etudiant.IdEtudiant)
                .ToListAsync();

            var model = new StudentDashboardViewModel
            {
                Etudiant = etudiant,

                LivresEmpruntes = emprunts.Count(e => e.DateRemiseEffective == null),

                ReservationsActives = reservations.Count(r =>
                    r.Statut == "En attente" || r.Statut == "Validée"),

                ScoreEmprunt = emprunts.Count,

                EmpruntsEnCours = emprunts
                    .Where(e => e.DateRemiseEffective == null)
                    .OrderBy(e => e.DateRemisePrevue)
                    .ToList(),

                Reservations = reservations
                    .OrderByDescending(r => r.DateReservation)
                    .Take(3)
                    .ToList(),

                HasAlertesProches = emprunts.Any(e =>
                    e.DateRemiseEffective == null &&
                    e.DateRemisePrevue >= today &&
                    e.DateRemisePrevue <= jPlus2)
            };

            return View(model);
        }
        public async Task<IActionResult> Search()
        {
            var model = new StudentBookSearchViewModel
            {
                Livres = await _context.Livres
                    .Include(l => l.Categorie)
                    .Include(l => l.AuteurLivres)
                        .ThenInclude(al => al.Auteur)
                    .ToListAsync(),

                Categories = await _context.Categories
                    .Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                    {
                        Value = c.IdCategorie.ToString(),
                        Text = c.NomCategorie
                    })
                    .ToListAsync()
            };

            return View(model);
        }

        public async Task<IActionResult> BookDetails(int id)
        {
            var livre = await _context.Livres
                .Include(l => l.Categorie)
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .FirstOrDefaultAsync(l => l.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        public async Task<IActionResult> BorrowBook(int id)
        {
            var email = User.Identity?.Name;

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FindAsync(id);

            if (livre == null || !livre.Disponible)
            {
                return RedirectToAction("Search");
            }

            var emprunt = new LIBRARY_PLUS.Models.Emprunt
            {
                IdLivre = livre.IdLivre,
                IdEtudiant = etudiant.IdEtudiant,
                DateEmprunt = DateTime.Today,
                DateRemisePrevue = DateTime.Today.AddDays(7),
                DateRemiseEffective = null,
                EstEnRetard = false,
                AlerteEnvoyee = false
            };

            livre.Disponible = false;

            _context.Emprunts.Add(emprunt);
            _context.Livres.Update(livre);

            await _context.SaveChangesAsync();

            return RedirectToAction("Emprunts");
        }

        public async Task<IActionResult> Emprunts()
        {
            var email = User.Identity?.Name;

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound();
            }

            var emprunts = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.IdEtudiant == etudiant.IdEtudiant)
                .OrderByDescending(e => e.DateEmprunt)
                .ToListAsync();

            return View(emprunts);
        }

        public async Task<IActionResult> ReserveBook(int id)
        {
            var livre = await _context.Livres
                .Include(l => l.Categorie)
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .FirstOrDefaultAsync(l => l.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReserveBook(int id, DateTime dateReservation)
        {
            var email = User.Identity?.Name;

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres.FindAsync(id);

            if (livre == null)
            {
                return NotFound();
            }

            if (dateReservation.Date < DateTime.Today)
            {
                ModelState.AddModelError("", "La date de réservation ne peut pas être dans le passé.");
                return View(livre);
            }

            var alreadyReserved = await _context.Reservations.AnyAsync(r =>
                r.IdEtudiant == etudiant.IdEtudiant &&
                r.IdLivre == livre.IdLivre &&
                (r.Statut == "En attente" || r.Statut == "Validée"));

            if (!alreadyReserved)
            {
                var reservation = new LIBRARY_PLUS.Models.Reservation
                {
                    IdLivre = livre.IdLivre,
                    IdEtudiant = etudiant.IdEtudiant,
                    DateReservation = dateReservation.Date,
                    Statut = "En attente"
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Reservations");
        }

        public async Task<IActionResult> Reservations()
        {
            var email = User.Identity?.Name;

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound();
            }

            var reservations = await _context.Reservations
                .Include(r => r.Livre)
                    .ThenInclude(l => l.Categorie)
                .Where(r => r.IdEtudiant == etudiant.IdEtudiant)
                .OrderByDescending(r => r.DateReservation)
                .ToListAsync();

            return View(reservations);
        }

        public async Task<IActionResult> CancelReservation(int id)
        {
            var email = User.Identity?.Name;

            var etudiant = await _context.Etudiants
                .FirstOrDefaultAsync(e => e.Email == email);

            if (etudiant == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r =>
                    r.IdReservation == id &&
                    r.IdEtudiant == etudiant.IdEtudiant);

            if (reservation == null)
            {
                return NotFound();
            }

            if (reservation.Statut == "En attente")
            {
                reservation.Statut = "Refusée";
                _context.Reservations.Update(reservation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Reservations");
        }

    }
}