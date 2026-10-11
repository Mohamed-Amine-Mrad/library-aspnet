using System.ComponentModel.DataAnnotations;

namespace LIBRARY_PLUS.Models
{
    public class Categorie
    {
        [Key]
        public int IdCategorie { get; set; }

        [Required]
        [StringLength(100)]
        public string NomCategorie { get; set; }

        [StringLength(250)]
        public string? Description { get; set; }

        public ICollection<Livre>? Livres { get; set; }
    }
}