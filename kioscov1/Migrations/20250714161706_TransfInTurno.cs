using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kioscov1.Migrations
{
    /// <inheritdoc />
    public partial class TransfInTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MontoTransferencia",
                table: "TurnosCaja",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MontoTransferencia",
                table: "TurnosCaja");
        }
    }
}
