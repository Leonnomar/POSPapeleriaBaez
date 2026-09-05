using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class AjustarControlHieleras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CantidadFiada",
                table: "DetalleSalidasHelados");

            migrationBuilder.DropColumn(
                name: "CantidadVendida",
                table: "DetalleSalidasHelados");

            migrationBuilder.RenameColumn(
                name: "Responsable",
                table: "SalidasHelados",
                newName: "TotalVendido");

            migrationBuilder.AddColumn<decimal>(
                name: "DineroRecibido",
                table: "SalidasHelados",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoPendiente",
                table: "SalidasHelados",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Precio",
                table: "DetalleSalidasHelados",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AbonosHielera",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SalidaHeladosId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Cantidad = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbonosHielera", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AbonosHielera_SalidasHelados_SalidaHeladosId",
                        column: x => x.SalidaHeladosId,
                        principalTable: "SalidasHelados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbonosHielera_SalidaHeladosId",
                table: "AbonosHielera",
                column: "SalidaHeladosId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbonosHielera");

            migrationBuilder.DropColumn(
                name: "DineroRecibido",
                table: "SalidasHelados");

            migrationBuilder.DropColumn(
                name: "SaldoPendiente",
                table: "SalidasHelados");

            migrationBuilder.DropColumn(
                name: "Precio",
                table: "DetalleSalidasHelados");

            migrationBuilder.RenameColumn(
                name: "TotalVendido",
                table: "SalidasHelados",
                newName: "Responsable");

            migrationBuilder.AddColumn<int>(
                name: "CantidadFiada",
                table: "DetalleSalidasHelados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CantidadVendida",
                table: "DetalleSalidasHelados",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
