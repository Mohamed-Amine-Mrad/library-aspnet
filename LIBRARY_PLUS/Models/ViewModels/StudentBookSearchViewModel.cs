using LIBRARY_PLUS.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LIBRARY_PLUS.Models.ViewModels
{
    public class StudentBookSearchViewModel
    {
        public List<Livre> Livres { get; set; } = new();

        public List<SelectListItem> Categories { get; set; } = new();

        public string? SearchTerm { get; set; }

        public int? IdCategorie { get; set; }
    }
}