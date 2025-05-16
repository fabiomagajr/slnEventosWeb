using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prjEventosWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class InscricoesAprimoradas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Inscricao_EventoId",
                table: "Inscricao");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Inscricao");

            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Inscricao");

            migrationBuilder.RenameColumn(
                name: "Vagas",
                table: "Evento",
                newName: "LimiteVagas");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Inscricao",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricao_ApplicationUserId",
                table: "Inscricao",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricao_EventoId_ApplicationUserId",
                table: "Inscricao",
                columns: new[] { "EventoId", "ApplicationUserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inscricao_AspNetUsers_ApplicationUserId",
                table: "Inscricao",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscricao_AspNetUsers_ApplicationUserId",
                table: "Inscricao");

            migrationBuilder.DropIndex(
                name: "IX_Inscricao_ApplicationUserId",
                table: "Inscricao");

            migrationBuilder.DropIndex(
                name: "IX_Inscricao_EventoId_ApplicationUserId",
                table: "Inscricao");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Inscricao");

            migrationBuilder.RenameColumn(
                name: "LimiteVagas",
                table: "Evento",
                newName: "Vagas");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Inscricao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Inscricao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Inscricao_EventoId",
                table: "Inscricao",
                column: "EventoId");
        }
    }
}
