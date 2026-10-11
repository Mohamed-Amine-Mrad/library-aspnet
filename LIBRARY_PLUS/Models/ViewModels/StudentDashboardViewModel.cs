using LIBRARY_PLUS.Models;

namespace LIBRARY_PLUS.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public Etudiant? Etudiant { get; set; }

        public int LivresEmpruntes { get; set; }
        public int ReservationsActives { get; set; }
        public int ScoreEmprunt { get; set; }

        public List<Emprunt> EmpruntsEnCours { get; set; } = new();
        public List<Reservation> Reservations { get; set; } = new();

        public bool HasAlertesProches { get; set; }
    }
}