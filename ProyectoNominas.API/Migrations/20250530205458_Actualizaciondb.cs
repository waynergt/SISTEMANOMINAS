using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoNominas.API.Migrations
{
    /// <inheritdoc />
    public partial class Actualizaciondb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nominas_Empleados_EmpleadoId",
                table: "Nominas");

            migrationBuilder.RenameColumn(
                name: "FechaPago",
                table: "Nominas",
                newName: "FechaInicio");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "Nominas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "Nominas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Periodo",
                table: "Nominas",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DetallesNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NominaId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HorasExtras = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Bonificaciones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Comisiones = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Descuentos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IGSS = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IRTRA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ISR = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPagar = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesNomina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesNomina_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesNomina_Nominas_NominaId",
                        column: x => x.NominaId,
                        principalTable: "Nominas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HorasTrabajadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HorasNormales = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    HorasExtras = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorasTrabajadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorasTrabajadas_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialAjustesNomina",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetalleNominaId = table.Column<int>(type: "int", nullable: false),
                    FechaAjuste = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValorAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorNuevo = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsuarioAjuste = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialAjustesNomina", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialAjustesNomina_DetallesNomina_DetalleNominaId",
                        column: x => x.DetalleNominaId,
                        principalTable: "DetallesNomina",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetallesNomina_EmpleadoId",
                table: "DetallesNomina",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesNomina_NominaId",
                table: "DetallesNomina",
                column: "NominaId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialAjustesNomina_DetalleNominaId",
                table: "HistorialAjustesNomina",
                column: "DetalleNominaId");

            migrationBuilder.CreateIndex(
                name: "IX_HorasTrabajadas_EmpleadoId",
                table: "HorasTrabajadas",
                column: "EmpleadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Nominas_Empleados_EmpleadoId",
                table: "Nominas",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nominas_Empleados_EmpleadoId",
                table: "Nominas");

            migrationBuilder.DropTable(
                name: "HistorialAjustesNomina");

            migrationBuilder.DropTable(
                name: "HorasTrabajadas");

            migrationBuilder.DropTable(
                name: "DetallesNomina");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "Nominas");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Nominas");

            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "Nominas",
                newName: "FechaPago");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "Nominas",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Nominas_Empleados_EmpleadoId",
                table: "Nominas",
                column: "EmpleadoId",
                principalTable: "Empleados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
