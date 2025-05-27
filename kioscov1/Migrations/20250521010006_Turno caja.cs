using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kioscov1.Migrations
{
    /// <inheritdoc />
    public partial class Turnocaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TurnoCajaId",
                table: "Ventas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TurnosCaja",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apertura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cierre = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MontoInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MontoFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TurnosCaja", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_TurnoCajaId",
                table: "Ventas",
                column: "TurnoCajaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_TurnosCaja_TurnoCajaId",
                table: "Ventas",
                column: "TurnoCajaId",
                principalTable: "TurnosCaja",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_TurnosCaja_TurnoCajaId",
                table: "Ventas");

            migrationBuilder.DropTable(
                name: "TurnosCaja");

            migrationBuilder.DropIndex(
                name: "IX_Ventas_TurnoCajaId",
                table: "Ventas");

            migrationBuilder.DropColumn(
                name: "TurnoCajaId",
                table: "Ventas");
        }
    }
}
