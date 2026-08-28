using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class AgregarUtilesDevolucionFabrica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaqueteCanjeId",
                table: "DevolucionesFabricaCanje",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DevolucionesFabricaCanje_PaqueteCanjeId",
                table: "DevolucionesFabricaCanje",
                column: "PaqueteCanjeId");

            migrationBuilder.AddForeignKey(
                name: "FK_DevolucionesFabricaCanje_PaquetesCanje_PaqueteCanjeId",
                table: "DevolucionesFabricaCanje",
                column: "PaqueteCanjeId",
                principalTable: "PaquetesCanje",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DevolucionesFabricaCanje_PaquetesCanje_PaqueteCanjeId",
                table: "DevolucionesFabricaCanje");

            migrationBuilder.DropIndex(
                name: "IX_DevolucionesFabricaCanje_PaqueteCanjeId",
                table: "DevolucionesFabricaCanje");

            migrationBuilder.DropColumn(
                name: "PaqueteCanjeId",
                table: "DevolucionesFabricaCanje");
        }
    }
}
