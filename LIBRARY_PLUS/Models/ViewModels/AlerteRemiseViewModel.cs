namespace LIBRARY_PLUS.Models.ViewModels
{
    public class AlerteRemiseViewModel
    {
        public string NomEtudiant { get; set; } = string.Empty;
        public string TitreLivre { get; set; } = string.Empty;
        public DateTime DateRemisePrevue { get; set; }
        public int JoursRestants { get; set; }
        public bool EstEnRetard { get; set; }
    }
}