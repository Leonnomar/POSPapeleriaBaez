using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class AjustarDevolucionClienteCanje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropForeignKey(
                name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropForeignKey(
                name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropForeignKey(
                name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropIndex(
                name: "IX_DevolucionesClienteCanje_TenisDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropIndex(
                name: "IX_DevolucionesClienteCanje_TenisEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropIndex(
                name: "IX_DevolucionesClienteCanje_UniformeDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropIndex(
                name: "IX_DevolucionesClienteCanje_UniformeEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "TenisDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "TenisEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "TipoArticulo",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "TipoCambio",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "UniformeDevueltoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.DropColumn(
                name: "UniformeEntregadoId",
                table: "DevolucionesClienteCanje");

            migrationBuilder.CreateTable(
                name: "DetalleDevolucionesClienteCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DevolucionClienteCanjeId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoCambio = table.Column<string>(type: "TEXT", nullable: false),
                    TipoArticulo = table.Column<string>(type: "TEXT", nullable: false),
                    UniformeDevueltoId = table.Column<int>(type: "INTEGER", nullable: true),
                    UniformeEntregadoId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisDevueltoId = table.Column<int>(type: "INTEGER", nullable: true),
                    TenisEntregadoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetalleDevolucionesClienteCanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionesClienteCanje_DevolucionesClienteCanje_DevolucionClienteCanjeId",
                        column: x => x.DevolucionClienteCanjeId,
                        principalTable: "DevolucionesClienteCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionesClienteCanje_TenisCanjes_TenisDevueltoId",
                        column: x => x.TenisDevueltoId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionesClienteCanje_TenisCanjes_TenisEntregadoId",
                        column: x => x.TenisEntregadoId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionesClienteCanje_UniformesCanje_UniformeDevueltoId",
                        column: x => x.UniformeDevueltoId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetalleDevolucionesClienteCanje_UniformesCanje_UniformeEntregadoId",
                        column: x => x.UniformeEntregadoId,
                        principalTable: "UniformesCanje",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleDevolucionesClienteCanje_DevolucionClienteCanjeId",
                table: "DetalleDevolucionesClienteCanje",
                column: "DevolucionClienteCanjeId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleDevolucionesClienteCanje_TenisDevueltoId",
                table: "DetalleDevolucionesClienteCanje",
                column: "TenisDevueltoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleDevolucionesClienteCanje_TenisEntregadoId",
                table: "DetalleDevolucionesClienteCanje",
                column: "TenisEntregadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleDevolucionesClienteCanje_UniformeDevueltoId",
                table: "DetalleDevolucionesClienteCanje",
                column: "UniformeDevueltoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleDevolucionesClienteCanje_UniformeEntregadoId",
                table: "DetalleDevolucionesClienteCanje",
                column: "UniformeEntregadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetalleDevolucionesClienteCanje");

            migrationBuilder.AddColumn<int>(
                name: "TenisDevueltoId",
                table: "DevolucionesClienteCanje",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenisEntregadoId",
                table: "DevolucionesClienteCanje",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoArticulo",
                table: "DevolucionesClienteCanje",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoCambio",
                table: "DevolucionesClienteCanje",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UniformeDevueltoId",
                table: "DevolucionesClienteCanje",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UniformeEntregadoId",
                table: "DevolucionesClienteCanje",
                type: "INTEGER",
                nullable: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisDevueltoId",
                table: "DevolucionesClienteCanje",
                column: "TenisDevueltoId",
                principalTable: "TenisCanjes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DevolucionesClienteCanje_TenisCanjes_TenisEntregadoId",
                table: "DevolucionesClienteCanje",
                column: "TenisEntregadoId",
                principalTable: "TenisCanjes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeDevueltoId",
                table: "DevolucionesClienteCanje",
                column: "UniformeDevueltoId",
                principalTable: "UniformesCanje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DevolucionesClienteCanje_UniformesCanje_UniformeEntregadoId",
                table: "DevolucionesClienteCanje",
                column: "UniformeEntregadoId",
                principalTable: "UniformesCanje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
