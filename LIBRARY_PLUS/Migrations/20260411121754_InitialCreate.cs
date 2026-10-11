using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIBRARY_PLUS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Auteurs",
                columns: table => new
                {
                    IdAuteur = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auteurs", x => x.IdAuteur);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    IdCategorie = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomCategorie = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.IdCategorie);
                });

            migrationBuilder.CreateTable(
                name: "Etudiants",
                columns: table => new
                {
                    IdEtudiant = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prenom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Classe = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ScoreEmprunt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etudiants", x => x.IdEtudiant);
                });

            migrationBuilder.CreateTable(
                name: "Livres",
                columns: table => new
                {
                    IdLivre = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnneePublication = table.Column<int>(type: "int", nullable: false),
                    Disponible = table.Column<bool>(type: "bit", nullable: false),
                    IdAuteur = table.Column<int>(type: "int", nullable: false),
                    IdCategorie = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Livres", x => x.IdLivre);
                    table.ForeignKey(
                        name: "FK_Livres_Auteurs_IdAuteur",
                        column: x => x.IdAuteur,
                        principalTable: "Auteurs",
                        principalColumn: "IdAuteur",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Livres_Categories_IdCategorie",
                        column: x => x.IdCategorie,
                        principalTable: "Categories",
                        principalColumn: "IdCategorie",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    IdReservation = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateReservation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdLivre = table.Column<int>(type: "int", nullable: false),
                    IdEtudiant = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.IdReservation);
                    table.ForeignKey(
                        name: "FK_Reservations_Etudiants_IdEtudiant",
                        column: x => x.IdEtudiant,
                        principalTable: "Etudiants",
                        principalColumn: "IdEtudiant",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Livres_IdLivre",
                        column: x => x.IdLivre,
                        principalTable: "Livres",
                        principalColumn: "IdLivre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Emprunts",
                columns: table => new
                {
                    IdEmprunt = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateEmprunt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRemisePrevue = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateRemiseEffective = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstEnRetard = table.Column<bool>(type: "bit", nullable: false),
                    AlerteEnvoyee = table.Column<bool>(type: "bit", nullable: false),
                    IdLivre = table.Column<int>(type: "int", nullable: false),
                    IdEtudiant = table.Column<int>(type: "int", nullable: false),
                    IdReservation = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprunts", x => x.IdEmprunt);
                    table.ForeignKey(
                        name: "FK_Emprunts_Etudiants_IdEtudiant",
                        column: x => x.IdEtudiant,
                        principalTable: "Etudiants",
                        principalColumn: "IdEtudiant",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emprunts_Livres_IdLivre",
                        column: x => x.IdLivre,
                        principalTable: "Livres",
                        principalColumn: "IdLivre",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Emprunts_Reservations_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "Reservations",
                        principalColumn: "IdReservation",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_IdEtudiant",
                table: "Emprunts",
                column: "IdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_IdLivre",
                table: "Emprunts",
                column: "IdLivre");

            migrationBuilder.CreateIndex(
                name: "IX_Emprunts_IdReservation",
                table: "Emprunts",
                column: "IdReservation",
                unique: true,
                filter: "[IdReservation] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Livres_IdAuteur",
                table: "Livres",
                column: "IdAuteur");

            migrationBuilder.CreateIndex(
                name: "IX_Livres_IdCategorie",
                table: "Livres",
                column: "IdCategorie");

            migrationBuilder.CreateIndex(
                name: "IX_Livres_ISBN",
                table: "Livres",
                column: "ISBN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdEtudiant",
                table: "Reservations",
                column: "IdEtudiant");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_IdLivre",
                table: "Reservations",
                column: "IdLivre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emprunts");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Etudiants");

            migrationBuilder.DropTable(
                name: "Livres");

            migrationBuilder.DropTable(
                name: "Auteurs");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
