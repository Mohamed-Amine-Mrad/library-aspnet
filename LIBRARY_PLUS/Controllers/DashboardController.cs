using LIBRARY_PLUS.Data;
using LIBRARY_PLUS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LIBRARY_PLUS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var aujourdHui = DateTime.Today;
            var dateJPlus2 = aujourdHui.AddDays(2);

            var dashboard = new DashboardViewModel
            {
                NombreLivres = await _context.Livres.CountAsync(),
                NombreAuteurs = await _context.Auteurs.CountAsync(),
                NombreCategories = await _context.Categories.CountAsync(),
                NombreEmprunts = await _context.Emprunts.CountAsync()
            };

            dashboard.EmpruntsParLivre = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.Livre != null)
                .GroupBy(e => e.Livre!.Titre)
                .Select(g => new StatistiqueLivreViewModel
                {
                    TitreLivre = g.Key,
                    NombreEmprunts = g.Count()
                })
                .OrderByDescending(x => x.NombreEmprunts)
                .ToListAsync();

            dashboard.Top5Livres = await _context.Emprunts
                .Include(e => e.Livre)
                .Where(e => e.Livre != null)
                .GroupBy(e => e.Livre!.Titre)
                .Select(g => new StatistiqueLivreViewModel
                {
                    TitreLivre = g.Key,
                    NombreEmprunts = g.Count()
                })
                .OrderByDescending(x => x.NombreEmprunts)
                .Take(5)
                .ToListAsync();

            dashboard.ScoringEtudiants = await _context.Emprunts
                .Include(e => e.Etudiant)
                .Where(e => e.Etudiant != null)
                .GroupBy(e => new
                {
                    e.Etudiant!.Nom,
                    e.Etudiant.Prenom,
                    e.Etudiant.Classe
                })
                .Select(g => new ScoringEtudiantViewModel
                {
                    NomComplet = g.Key.Nom + " " + g.Key.Prenom,
                    Classe = g.Key.Classe,

                    NombreEmprunts = g.Count(),

                    NombreEmpruntsReglesDansLesDelais = g.Count(e =>
                        e.DateRemiseEffective.HasValue &&
                        e.DateRemiseEffective.Value.Date <= e.DateRemisePrevue.Date),

                    NiveauScore = g.Count() >= 25 ? "Excellent" :
                                  g.Count() >= 20 ? "Très bien" :
                                  g.Count() >= 15 ? "Bien" : "Moyen"
                })
                .OrderByDescending(x => x.NombreEmprunts)
                .Take(10)
                .ToListAsync();

            dashboard.AlertesRemisesProches = await _context.Emprunts
                .Include(e => e.Etudiant)
                .Include(e => e.Livre)
                .Where(e =>
                    e.DateRemiseEffective == null &&
                    e.DateRemisePrevue >= aujourdHui &&
                    e.DateRemisePrevue <= dateJPlus2)
                .Select(e => new AlerteRemiseViewModel
                {
                    NomEtudiant = e.Etudiant != null ? e.Etudiant.Nom + " " + e.Etudiant.Prenom : "",
                    TitreLivre = e.Livre != null ? e.Livre.Titre : "",
                    DateRemisePrevue = e.DateRemisePrevue,
                    JoursRestants = EF.Functions.DateDiffDay(aujourdHui, e.DateRemisePrevue),
                    EstEnRetard = false
                })
                .OrderBy(x => x.DateRemisePrevue)
                .ToListAsync();

            dashboard.EmpruntsEnRetard = await _context.Emprunts
                .Include(e => e.Etudiant)
                .Include(e => e.Livre)
                .Where(e =>
                    e.DateRemiseEffective == null &&
                    e.DateRemisePrevue < aujourdHui)
                .Select(e => new AlerteRemiseViewModel
                {
                    NomEtudiant = e.Etudiant != null ? e.Etudiant.Nom + " " + e.Etudiant.Prenom : "",
                    TitreLivre = e.Livre != null ? e.Livre.Titre : "",
                    DateRemisePrevue = e.DateRemisePrevue,
                    JoursRestants = EF.Functions.DateDiffDay(e.DateRemisePrevue, aujourdHui),
                    EstEnRetard = true
                })
                .OrderByDescending(x => x.JoursRestants)
                .ToListAsync();

            return View(dashboard);
        }
    }
}