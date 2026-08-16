using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaBaez.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSaldoPendienteDeuda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeldoPendiente",
                table: "Deudas",
                newName: "SaldoPendiente");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SaldoPendiente",
                table: "Deudas",
                newName: "SeldoPendiente");
        }
    }
}
