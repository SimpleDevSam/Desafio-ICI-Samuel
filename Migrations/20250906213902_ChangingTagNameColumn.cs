using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Desafio_ICI_Samuel.Migrations
{
    /// <inheritdoc />
    public partial class ChangingTagNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Tags",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tags",
                newName: "Nome");
        }
    }
}
