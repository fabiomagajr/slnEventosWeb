using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace prjEventosWeb.Data.Migrations
{
    /// <inheritdoc />
    public partial class advanc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeCompleto",
                table: "AspNetUsers",
                newName: "NomeUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NomeUser",
                table: "AspNetUsers",
                newName: "NomeCompleto");
        }
    }
}
