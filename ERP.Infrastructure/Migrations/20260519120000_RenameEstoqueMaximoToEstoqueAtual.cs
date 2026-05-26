using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Infrastructure.Migrations
{
    public partial class RenameEstoqueMaximoToEstoqueAtual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstoqueMaximo",
                table: "Produtos",
                newName: "EstoqueAtual");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EstoqueAtual",
                table: "Produtos",
                newName: "EstoqueMaximo");
        }
    }
}