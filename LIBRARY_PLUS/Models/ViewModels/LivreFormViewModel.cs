using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LIBRARY_PLUS.Models.ViewModels
{
    public class LivreFormViewModel
    {
        public int IdLivre { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ISBN { get; set; } = string.Empty;

        [Range(1000, 9999)]
        public int AnneePublication { get; set; }

        public bool Disponible { get; set; } = true;

        [Required]
        public int IdCategorie { get; set; }

        public List<int> SelectedAuteurIds { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Auteurs { get; set; } = new();
    }
}