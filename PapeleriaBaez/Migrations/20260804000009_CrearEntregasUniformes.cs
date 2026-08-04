using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearEntregasUniformes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CanjeUniformes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CantidadConjuntos = table.Column<int>(type: "INTEGER", nullable: false),
                    Observaciones = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanjeUniformes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetalleCanjeUniformes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CanjeUniformeId = table.Column<int>(type: "INTEGER", nullable: false),
                    UniformeCanjeId = table.Column<int>(type: "INTEGER", nullable: false),
                    NumeroConjunto = table.Column<int>(type: "INTEGER", nullable: false),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    Pendiente = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleCanjeUniformes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleCanjeUniformes_CanjeUniformes_CanjeUniformeId",
                        column: x => x.CanjeUniformeId,
                        principalTable: "CanjeUniformes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleCanjeUniformes_UniformesCanje_UniformeCanjeId",
                        column: x => x.UniformeCanjeId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCanjeUniformes_CanjeUniformeId",
                table: "DetalleCanjeUniformes",
                column: "CanjeUniformeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleCanjeUniformes_UniformeCanjeId",
                table: "DetalleCanjeUniformes",
                column: "UniformeCanjeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleCanjeUniformes");

            migrationBuilder.DropTable(
                name: "CanjeUniformes");
        }
    }
}
