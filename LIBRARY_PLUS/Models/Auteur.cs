using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class Auteur
    {
        [Key]
        public int IdAuteur { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; } = string.Empty;

        [NotMapped]
        public string NomComplet => Nom + " " + Prenom;

        public ICollection<AuteurLivre>? AuteurLivres { get; set; }
    }
}