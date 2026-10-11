using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class Etudiant
    {
        [Key]
        public int IdEtudiant { get; set; }

        [Required]
        [StringLength(100)]
        public string Nom { get; set; }

        [Required]
        [StringLength(100)]
        public string Prenom { get; set; }

        [NotMapped]
        public string NomComplet => Nom + " " + Prenom;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Classe { get; set; }

        public int ScoreEmprunt { get; set; } = 0;

        public ICollection<Reservation>? Reservations { get; set; }
        public ICollection<Emprunt>? Emprunts { get; set; }
    }
}