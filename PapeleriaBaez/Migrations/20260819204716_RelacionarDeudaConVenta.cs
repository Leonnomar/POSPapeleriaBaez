using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class RelacionarDeudaConVenta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VentaId",
                table: "Deudas",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deudas_VentaId",
                table: "Deudas",
                column: "VentaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deudas_Ventas_VentaId",
                table: "Deudas",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deudas_Ventas_VentaId",
                table: "Deudas");

            migrationBuilder.DropIndex(
                name: "IX_Deudas_VentaId",
                table: "Deudas");

            migrationBuilder.DropColumn(
                name: "VentaId",
                table: "Deudas");
        }
    }
}
