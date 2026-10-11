using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class Emprunt
    {
        [Key]
        public int IdEmprunt { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateEmprunt { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateRemisePrevue { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateRemiseEffective { get; set; }

        public bool EstEnRetard { get; set; } = false;

        public bool AlerteEnvoyee { get; set; } = false;

        [Display(Name = "Livre")]
        public int IdLivre { get; set; }

        [ForeignKey("IdLivre")]
        public Livre? Livre { get; set; }

        [Display(Name = "Etudiant")]
        public int IdEtudiant { get; set; }

        [ForeignKey("IdEtudiant")]
        public Etudiant? Etudiant { get; set; }

        [Display(Name = "Reservation")]
        public int? IdReservation { get; set; }

        [ForeignKey("IdReservation")]
        public Reservation? Reservation { get; set; }
    }
}