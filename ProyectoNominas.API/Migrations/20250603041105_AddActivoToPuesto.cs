using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoNominas.API.Migrations
{
    /// <inheritdoc />
    public partial class AddActivoToPuesto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "Puestos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Activo",
                table: "Puestos");
        }
    }
}
