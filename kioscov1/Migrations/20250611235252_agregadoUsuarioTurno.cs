using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kioscov1.Migrations
{
    /// <inheritdoc />
    public partial class agregadoUsuarioTurno : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioId1",
                table: "TurnosCaja",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TurnosCaja_UsuarioId1",
                table: "TurnosCaja",
                column: "UsuarioId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TurnosCaja_Usuarios_UsuarioId1",
                table: "TurnosCaja",
                column: "UsuarioId1",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TurnosCaja_Usuarios_UsuarioId1",
                table: "TurnosCaja");

            migrationBuilder.DropIndex(
                name: "IX_TurnosCaja_UsuarioId1",
                table: "TurnosCaja");

            migrationBuilder.DropColumn(
                name: "UsuarioId1",
                table: "TurnosCaja");
        }
    }
}
