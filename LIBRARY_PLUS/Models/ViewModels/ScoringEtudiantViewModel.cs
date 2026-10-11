namespace LIBRARY_PLUS.Models.ViewModels
{
    public class ScoringEtudiantViewModel
    {
        public string NomComplet { get; set; } = string.Empty;
        public string Classe { get; set; } = string.Empty;
        public int NombreEmprunts { get; set; }
        public int NombreEmpruntsReglesDansLesDelais { get; set; }
        public string NiveauScore { get; set; } = string.Empty;
    }
}