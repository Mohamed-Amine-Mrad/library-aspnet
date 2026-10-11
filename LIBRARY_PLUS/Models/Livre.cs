using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class Livre
    {
        [Key]
        public int IdLivre { get; set; }

        [Required]
        [StringLength(200)]
        public string Titre { get; set; }

        [Required]
        [StringLength(50)]
        public string ISBN { get; set; }

        [Range(1000, 9999)]
        public int AnneePublication { get; set; }

        public bool Disponible { get; set; } = true;

        public int IdCategorie { get; set; }

        [ForeignKey("IdCategorie")]
        public Categorie? Categorie { get; set; }

        public ICollection<AuteurLivre>? AuteurLivres { get; set; }
        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Emprunt>? Emprunts { get; set; }
    }
}