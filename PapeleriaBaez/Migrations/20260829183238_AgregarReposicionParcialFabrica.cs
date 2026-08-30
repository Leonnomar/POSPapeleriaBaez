using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class AgregarReposicionParcialFabrica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CantidadFinal",
                table: "DevolucionesFabricaCanje",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadRespuesta",
                table: "DevolucionesFabricaCanje",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadFinal",
                table: "DevolucionesFabricaCanje");

            migrationBuilder.DropColumn(
                name: "CantidadRespuesta",
                table: "DevolucionesFabricaCanje");
        }
    }
}
