using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class Reservation
    {
        [Key]
        public int IdReservation { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateReservation { get; set; } = DateTime.Today;

        [Required]
        [StringLength(50)]
        public string Statut { get; set; } = "En attente";

        [Display(Name = "Livre")]
        public int IdLivre { get; set; }

        [ForeignKey("IdLivre")]
        public Livre? Livre { get; set; }

        [Display(Name = "Etudiant")]
        public int IdEtudiant { get; set; }

        [ForeignKey("IdEtudiant")]
        public Etudiant? Etudiant { get; set; }

        public Emprunt? Emprunt { get; set; }
    }
}