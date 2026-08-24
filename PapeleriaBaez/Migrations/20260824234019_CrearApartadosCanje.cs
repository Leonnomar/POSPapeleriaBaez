using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearApartadosCanje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApartadosCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Referencia = table.Column<string>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApartadosCanje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetalleApartadosCanjes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ApartadoCanjeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    PaqueteCanjeId = table.Column<int>(type: "INTEGER", nullable: true),
                    UniformeCanjeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisCanjeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleApartadosCanjes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleApartadosCanjes_ApartadosCanje_ApartadoCanjeId",
                        column: x => x.ApartadoCanjeId,
                        principalTable: "ApartadosCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleApartadosCanjes_PaquetesCanje_PaqueteCanjeId",
                        column: x => x.PaqueteCanjeId,
                        principalTable: "PaquetesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleApartadosCanjes_TenisCanjes_TenisCanjeId",
                        column: x => x.TenisCanjeId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleApartadosCanjes_UniformesCanje_UniformeCanjeId",
                        column: x => x.UniformeCanjeId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleApartadosCanjes_ApartadoCanjeId",
                table: "DetalleApartadosCanjes",
                column: "ApartadoCanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleApartadosCanjes_PaqueteCanjeId",
                table: "DetalleApartadosCanjes",
                column: "PaqueteCanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleApartadosCanjes_TenisCanjeId",
                table: "DetalleApartadosCanjes",
                column: "TenisCanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleApartadosCanjes_UniformeCanjeId",
                table: "DetalleApartadosCanjes",
                column: "UniformeCanjeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleApartadosCanjes");

            migrationBuilder.DropTable(
                name: "ApartadosCanje");
        }
    }
}
