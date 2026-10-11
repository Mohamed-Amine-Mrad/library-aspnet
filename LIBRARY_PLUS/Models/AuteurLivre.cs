using System.ComponentModel.DataAnnotations.Schema;

namespace LIBRARY_PLUS.Models
{
    public class AuteurLivre
    {
        public int IdAuteur { get; set; }
        public Auteur? Auteur { get; set; }

        public int IdLivre { get; set; }
        public Livre? Livre { get; set; }
    }
}