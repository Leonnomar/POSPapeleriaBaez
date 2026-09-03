using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearSalidasHelados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalidasHelados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FechaSalida = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Responsable = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalidasHelados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetalleSalidasHelados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SalidaHeladosId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProductoId = table.Column<int>(type: "INTEGER", nullable: false),
                    CantidadSalida = table.Column<int>(type: "INTEGER", nullable: false),
                    CantidadRegresada = table.Column<int>(type: "INTEGER", nullable: false),
                    CantidadVendida = table.Column<int>(type: "INTEGER", nullable: false),
                    CantidadFiada = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleSalidasHelados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleSalidasHelados_Productos_ProductoId",
                        column: x => x.ProductoId,
                        principalTable: "Productos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleSalidasHelados_SalidasHelados_SalidaHeladosId",
                        column: x => x.SalidaHeladosId,
                        principalTable: "SalidasHelados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleSalidasHelados_ProductoId",
                table: "DetalleSalidasHelados",
                column: "ProductoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleSalidasHelados_SalidaHeladosId",
                table: "DetalleSalidasHelados",
                column: "SalidaHeladosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleSalidasHelados");

            migrationBuilder.DropTable(
                name: "SalidasHelados");
        }
    }
}
