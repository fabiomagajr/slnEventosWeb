using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prjEventosWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class NewFunctionsNN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "AspNetUsers",
                newName: "NomeCompleto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeCompleto",
                table: "AspNetUsers",
                newName: "Nome");
        }
    }
}
