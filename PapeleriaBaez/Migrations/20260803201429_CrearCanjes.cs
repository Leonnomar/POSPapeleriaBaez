using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearCanjes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaquetesCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NumeroPaquete = table.Column<int>(type: "INTEGER", nullable: false),
                    Existencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Entregados = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaquetesCanje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UniformesCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Talla = table.Column<string>(type: "TEXT", nullable: false),
                    Existencia = table.Column<int>(type: "INTEGER", nullable: false),
                    Entregados = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniformesCanje", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaquetesCanje");

            migrationBuilder.DropTable(
                name: "UniformesCanje");
        }
    }
}
