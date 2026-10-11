using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LIBRARY_PLUS.Migrations
{
    /// <inheritdoc />
    public partial class AddAuteurLivreRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Livres_Auteurs_IdAuteur",
                table: "Livres");

            migrationBuilder.DropIndex(
                name: "IX_Livres_IdAuteur",
                table: "Livres");

            migrationBuilder.DropColumn(
                name: "IdAuteur",
                table: "Livres");

            migrationBuilder.CreateTable(
                name: "AuteurLivres",
                columns: table => new
                {
                    IdAuteur = table.Column<int>(type: "int", nullable: false),
                    IdLivre = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuteurLivres", x => new { x.IdAuteur, x.IdLivre });
                    table.ForeignKey(
                        name: "FK_AuteurLivres_Auteurs_IdAuteur",
                        column: x => x.IdAuteur,
                        principalTable: "Auteurs",
                        principalColumn: "IdAuteur",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuteurLivres_Livres_IdLivre",
                        column: x => x.IdLivre,
                        principalTable: "Livres",
                        principalColumn: "IdLivre",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuteurLivres_IdLivre",
                table: "AuteurLivres",
                column: "IdLivre");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuteurLivres");

            migrationBuilder.AddColumn<int>(
                name: "IdAuteur",
                table: "Livres",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Livres_IdAuteur",
                table: "Livres",
                column: "IdAuteur");

            migrationBuilder.AddForeignKey(
                name: "FK_Livres_Auteurs_IdAuteur",
                table: "Livres",
                column: "IdAuteur",
                principalTable: "Auteurs",
                principalColumn: "IdAuteur",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
