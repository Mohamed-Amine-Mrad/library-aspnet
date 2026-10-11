using LIBRARY_PLUS.Data;
using LIBRARY_PLUS.Models;
using LIBRARY_PLUS.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LIBRARY_PLUS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LivresController : Controller
    {
        private readonly AppDbContext _context;

        public LivresController(AppDbContext context)
        {
            _context = context;
        }

        // ================= INDEX =================
        public async Task<IActionResult> Index(
            string searchTerm,
            int? auteurId,
            int? categorieId,
            bool? disponible)
        {
            var query = _context.Livres
                .Include(l => l.Categorie)
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(l =>
                    l.Titre.Contains(searchTerm) ||
                    l.ISBN.Contains(searchTerm));
            }

            if (auteurId.HasValue)
            {
                query = query.Where(l =>
                    l.AuteurLivres.Any(al => al.IdAuteur == auteurId));
            }

            if (categorieId.HasValue)
            {
                query = query.Where(l =>
                    l.IdCategorie == categorieId);
            }

            if (disponible.HasValue)
            {
                query = query.Where(l =>
                    l.Disponible == disponible);
            }

            ViewBag.Auteurs = await _context.Auteurs
                .Select(a => new SelectListItem
                {
                    Value = a.IdAuteur.ToString(),
                    Text = a.Nom + " " + a.Prenom
                }).ToListAsync();

            ViewBag.Categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.IdCategorie.ToString(),
                    Text = c.NomCategorie
                }).ToListAsync();

            return View(await query.ToListAsync());
        }

        // ================= DETAILS =================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.Categorie)
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .FirstOrDefaultAsync(m => m.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // ================= CREATE GET =================
        public async Task<IActionResult> Create()
        {
            var viewModel = new LivreFormViewModel();

            await ChargerListesDeroulantes(viewModel);

            return View(viewModel);
        }

        // ================= CREATE POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LivreFormViewModel viewModel)
        {
            if (viewModel.SelectedAuteurIds == null || !viewModel.SelectedAuteurIds.Any())
            {
                ModelState.AddModelError("SelectedAuteurIds",
                    "Veuillez sélectionner au moins un auteur.");
            }

            var isbnExiste = await _context.Livres
                .AnyAsync(l => l.ISBN == viewModel.ISBN);

            if (isbnExiste)
            {
                ModelState.AddModelError("ISBN",
                    "Ce ISBN existe déjà. Veuillez saisir un ISBN unique.");
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(viewModel);
                return View(viewModel);
            }

            var livre = new Livre
            {
                Titre = viewModel.Titre,
                ISBN = viewModel.ISBN,
                AnneePublication = viewModel.AnneePublication,
                Disponible = viewModel.Disponible,
                IdCategorie = viewModel.IdCategorie,
                AuteurLivres = new List<AuteurLivre>()
            };

            foreach (var auteurId in viewModel.SelectedAuteurIds)
            {
                livre.AuteurLivres.Add(new AuteurLivre
                {
                    IdAuteur = auteurId
                });
            }

            _context.Add(livre);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ================= EDIT GET =================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.AuteurLivres)
                .FirstOrDefaultAsync(l => l.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            var viewModel = new LivreFormViewModel
            {
                IdLivre = livre.IdLivre,
                Titre = livre.Titre,
                ISBN = livre.ISBN,
                AnneePublication = livre.AnneePublication,
                Disponible = livre.Disponible,
                IdCategorie = livre.IdCategorie,
                SelectedAuteurIds = livre.AuteurLivres
                    .Select(al => al.IdAuteur)
                    .ToList()
            };

            await ChargerListesDeroulantes(viewModel);

            return View(viewModel);
        }

        // ================= EDIT POST =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LivreFormViewModel viewModel)
        {
            if (id != viewModel.IdLivre)
            {
                return NotFound();
            }

            if (viewModel.SelectedAuteurIds == null || !viewModel.SelectedAuteurIds.Any())
            {
                ModelState.AddModelError("SelectedAuteurIds",
                    "Veuillez sélectionner au moins un auteur.");
            }

            var isbnExiste = await _context.Livres
                .AnyAsync(l => l.ISBN == viewModel.ISBN &&
                               l.IdLivre != viewModel.IdLivre);

            if (isbnExiste)
            {
                ModelState.AddModelError("ISBN",
                    "Ce ISBN existe déjà. Veuillez saisir un ISBN unique.");
            }

            if (!ModelState.IsValid)
            {
                await ChargerListesDeroulantes(viewModel);
                return View(viewModel);
            }

            var livre = await _context.Livres
                .Include(l => l.AuteurLivres)
                .FirstOrDefaultAsync(l => l.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            livre.Titre = viewModel.Titre;
            livre.ISBN = viewModel.ISBN;
            livre.AnneePublication = viewModel.AnneePublication;
            livre.Disponible = viewModel.Disponible;
            livre.IdCategorie = viewModel.IdCategorie;

            _context.AuteurLivres.RemoveRange(livre.AuteurLivres);

            livre.AuteurLivres = new List<AuteurLivre>();

            foreach (var auteurId in viewModel.SelectedAuteurIds)
            {
                livre.AuteurLivres.Add(new AuteurLivre
                {
                    IdLivre = livre.IdLivre,
                    IdAuteur = auteurId
                });
            }

            try
            {
                _context.Update(livre);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivreExists(viewModel.IdLivre))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= DELETE GET =================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livre = await _context.Livres
                .Include(l => l.Categorie)
                .Include(l => l.AuteurLivres)
                    .ThenInclude(al => al.Auteur)
                .FirstOrDefaultAsync(m => m.IdLivre == id);

            if (livre == null)
            {
                return NotFound();
            }

            return View(livre);
        }

        // ================= DELETE POST =================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var livre = await _context.Livres
                .Include(l => l.AuteurLivres)
                .FirstOrDefaultAsync(l => l.IdLivre == id);

            if (livre != null)
            {
                _context.AuteurLivres.RemoveRange(livre.AuteurLivres);

                _context.Livres.Remove(livre);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ================= HELPERS =================
        private async Task ChargerListesDeroulantes(LivreFormViewModel viewModel)
        {
            viewModel.Categories = await _context.Categories
                .Select(c => new SelectListItem
                {
                    Value = c.IdCategorie.ToString(),
                    Text = c.NomCategorie
                }).ToListAsync();

            viewModel.Auteurs = await _context.Auteurs
                .Select(a => new SelectListItem
                {
                    Value = a.IdAuteur.ToString(),
                    Text = a.Nom + " " + a.Prenom
                }).ToListAsync();
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.IdLivre == id);
        }
    }
}