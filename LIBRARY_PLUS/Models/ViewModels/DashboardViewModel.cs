namespace LIBRARY_PLUS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int NombreLivres { get; set; }
        public int NombreAuteurs { get; set; }
        public int NombreCategories { get; set; }
        public int NombreEmprunts { get; set; }

        public List<StatistiqueLivreViewModel> EmpruntsParLivre { get; set; } = new();
        public List<StatistiqueLivreViewModel> Top5Livres { get; set; } = new();
        public List<ScoringEtudiantViewModel> ScoringEtudiants { get; set; } = new();
        public List<AlerteRemiseViewModel> AlertesRemisesProches { get; set; } = new();
        public List<AlerteRemiseViewModel> EmpruntsEnRetard { get; set; } = new();
    }
}