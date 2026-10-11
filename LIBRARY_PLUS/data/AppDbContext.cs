using LIBRARY_PLUS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LIBRARY_PLUS.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Auteur> Auteurs { get; set; }
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Livre> Livres { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Emprunt> Emprunts { get; set; }
        public DbSet<AuteurLivre> AuteurLivres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Livre>()
                .HasIndex(l => l.ISBN)
                .IsUnique();

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Emprunt)
                .WithOne(e => e.Reservation)
                .HasForeignKey<Emprunt>(e => e.IdReservation)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AuteurLivre>()
                .HasKey(al => new { al.IdAuteur, al.IdLivre });

            modelBuilder.Entity<AuteurLivre>()
                .HasOne(al => al.Auteur)
                .WithMany(a => a.AuteurLivres)
                .HasForeignKey(al => al.IdAuteur);

            modelBuilder.Entity<AuteurLivre>()
                .HasOne(al => al.Livre)
                .WithMany(l => l.AuteurLivres)
                .HasForeignKey(al => al.IdLivre);
        }
    }
}