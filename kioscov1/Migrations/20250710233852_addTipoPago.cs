using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kioscov1.Migrations
{
    /// <inheritdoc />
    public partial class addTipoPago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoPago",
                table: "Ventas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoPago",
                table: "Ventas");
        }
    }
}
