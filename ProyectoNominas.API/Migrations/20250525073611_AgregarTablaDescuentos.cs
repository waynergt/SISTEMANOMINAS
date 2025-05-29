using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoNominas.API.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTablaDescuentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DescuentosLegales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescuentosLegales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DetallesDescuentoNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NominaId = table.Column<int>(type: "int", nullable: false),
                    DescuentoLegalId = table.Column<int>(type: "int", nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesDescuentoNomina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesDescuentoNomina_DescuentosLegales_DescuentoLegalId",
                        column: x => x.DescuentoLegalId,
                        principalTable: "DescuentosLegales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesDescuentoNomina_Nominas_NominaId",
                        column: x => x.NominaId,
                        principalTable: "Nominas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDescuentoNomina_DescuentoLegalId",
                table: "DetallesDescuentoNomina",
                column: "DescuentoLegalId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDescuentoNomina_NominaId",
                table: "DetallesDescuentoNomina",
                column: "NominaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesDescuentoNomina");

            migrationBuilder.DropTable(
                name: "DescuentosLegales");
        }
    }
}
