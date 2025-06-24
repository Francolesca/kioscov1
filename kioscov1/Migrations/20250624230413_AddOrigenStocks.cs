using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kioscov1.Migrations
{
    /// <inheritdoc />
    public partial class AddOrigenStocks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comentario",
                table: "MovimientosStock",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "MovimientosStock",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comentario",
                table: "MovimientosStock");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "MovimientosStock");
        }
    }
}
