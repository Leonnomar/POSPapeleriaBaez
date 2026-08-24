using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class CrearValesTenisCanje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ValesTenisCanje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TenisCanjeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Pendiente = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValesTenisCanje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValesTenisCanje_TenisCanjes_TenisCanjeId",
                        column: x => x.TenisCanjeId,
                        principalTable: "TenisCanjes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ValesTenisCanje_TenisCanjeId",
                table: "ValesTenisCanje",
                column: "TenisCanjeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ValesTenisCanje");
        }
    }
}
