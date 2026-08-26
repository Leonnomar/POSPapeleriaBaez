using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearDevolucionesCanjeV11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DevolucionesClienteCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoCambio = table.Column<string>(type: "TEXT", nullable: false),
                    TipoArticulo = table.Column<string>(type: "TEXT", nullable: false),
                    UniformeDevueltoId = table.Column<int>(type: "INTEGER", nullable: true),
                    UniformeEntregadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisDevueltoId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisEntregadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevolucionesClienteCanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisDevueltoId",
                        column: x => x.TenisDevueltoId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisEntregadoId",
                        column: x => x.TenisEntregadoId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeDevueltoId",
                        column: x => x.UniformeDevueltoId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeEntregadoId",
                        column: x => x.UniformeEntregadoId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevolucionesFabricaCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TipoDevolucion = table.Column<string>(type: "TEXT", nullable: false),
                    TipoArticulo = table.Column<string>(type: "TEXT", nullable: false),
                    UniformeCanjeId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisCanjeId = table.Column<int>(type: "INTEGER", nullable: true),
                    Cantidad = table.Column<int>(type: "INTEGER", nullable: false),
                    EstadoReposicion = table.Column<string>(type: "TEXT", nullable: false),
                    FechaReposicion = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Observacion = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevolucionesFabricaCanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DevolucionesFabricaCanje_TenisCanjes_TenisCanjeId",
                        column: x => x.TenisCanjeId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevolucionesFabricaCanje_UniformesCanje_UniformeCanjeId",
                        column: x => x.UniformeCanjeId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClienteCanje_TenisDevueltoId",
                table: "DevolucionesClienteCanje",
                column: "TenisDevueltoId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClienteCanje_TenisEntregadoId",
                table: "DevolucionesClienteCanje",
                column: "TenisEntregadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClienteCanje_UniformeDevueltoId",
                table: "DevolucionesClienteCanje",
                column: "UniformeDevueltoId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesClienteCanje_UniformeEntregadoId",
                table: "DevolucionesClienteCanje",
                column: "UniformeEntregadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesFabricaCanje_TenisCanjeId",
                table: "DevolucionesFabricaCanje",
                column: "TenisCanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesFabricaCanje_UniformeCanjeId",
                table: "DevolucionesFabricaCanje",
                column: "UniformeCanjeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DevolucionesClienteCanje");

            migrationBuilder.DropTable(
                name: "DevolucionesFabricaCanje");
        }
    }
}
