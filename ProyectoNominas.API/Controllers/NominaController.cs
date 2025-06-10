using Microsoft.AspNetCore.Mvc;
using ProyectoNominas.API.Services;
using ProyectoNominas.API.Domain.Entities;
using QuestPDF.Fluent;
using System.Globalization;
using QuestPDF.Helpers;
using ProyectoNominas.API.DTOs;
using QuestPDF.Infrastructure;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominaController : ControllerBase
    {
        private readonly NominaService _service;

        public NominaController(NominaService service)
        {
            _service = service;
        }

        // POST: api/Nomina/generar
        [HttpPost("generar")]
        public async Task<ActionResult<Nomina>> GenerarNomina([FromBody] GenerarNominaRequest request)
        {
            var nomina = await _service.GenerarNominaAsync(
                request.FechaInicio,
                request.FechaFin,
                request.Periodo,
                request.EmpleadoId // << nuevo parámetro
            );
            return Ok(nomina);
        }

        // GET: api/Nomina/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Nomina>> GetNomina(int id)
        {
            var nomina = await _service.ObtenerNominaPorIdAsync(id);

            if (nomina == null)
                return NotFound();

            return Ok(nomina);
        }

        // GET: api/Nomina
        [HttpGet]
        public async Task<ActionResult<List<Nomina>>> GetNominas()
        {
            var nominas = await _service.ListarNominasAsync();
            return Ok(nominas);
        }

        // GET: api/Nomina/recibo-pdf
        [HttpGet("recibo-pdf")]
        public IActionResult DescargarReciboPdf([FromQuery] int nominaId, [FromQuery] int empleadoId)
        {
            var model = new ReciboNominaDto
            {
                NombreEmpleado = "Ana Gómez",
                SalarioBase = 4000m,
                HorasExtras = 0m,
                Bonificaciones = 0m,
                Descuentos = 0m,
                IGSS = 193.20m,
                IRTRA = 40.00m,
                ISR = 200.00m,
                TotalPagar = 3566.80m,
                Moneda = "GTQ"
            };

            var pdf = GenerarReciboPDF(model);
            var nombreArchivo = $"Recibo_Nomina_{nominaId}_Empleado_{empleadoId}.pdf";
            return File(pdf, "application/pdf", nombreArchivo);
        }

        // GET: api/Nomina/descargar-completa
        [HttpGet("descargar-completa")]
        public async Task<IActionResult> DescargarNominaCompleta([FromQuery] int nominaId)
        {
            // Obtén la nómina y sus detalles
            var nomina = await _service.ObtenerNominaPorIdAsync(nominaId);
            if (nomina == null)
                return NotFound();

            // Genera el PDF de la nómina completa
            var pdf = GenerarPdfNominaCompleta(nomina);

            var nombreArchivo = $"Nomina_Completa_{nomina.Periodo}.pdf";
            return File(pdf, "application/pdf", nombreArchivo);
        }

        // ========== PDF HELPERS ==========

        public class ReciboNominaDto
        {
            public string NombreEmpleado { get; set; } = "";
            public decimal SalarioBase { get; set; }
            public decimal HorasExtras { get; set; }
            public decimal Bonificaciones { get; set; }
            public decimal Descuentos { get; set; }
            public decimal IGSS { get; set; }
            public decimal IRTRA { get; set; }
            public decimal ISR { get; set; }
            public decimal TotalPagar { get; set; }
            public string Moneda { get; set; } = "GTQ";
        }

        private byte[] GenerarReciboPDF(ReciboNominaDto model)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A5.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    // HEADER
                    page.Header()
                        .PaddingBottom(10)
                        .Row(r =>
                        {
                            r.RelativeItem().Text($"Recibo de Nómina - {model.NombreEmpleado}")
                                .FontColor("#233a7b").SemiBold().FontSize(18);
                        });

                    // CONTENT
                    page.Content().Element(c =>
                    {
                        c.Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            void AddRow(string concepto, string? valor, bool bold = false, string color = "#233a7b")
                            {
                                table.Cell().Border(1).BorderColor("#e4e7ef")
                                    .Background("#f8fafc")
                                    .Padding(8)
                                    .Text(concepto).FontColor(color).SemiBold();

                                table.Cell().Border(1).BorderColor("#e4e7ef")
                                    .Padding(8)
                                    .Text(t =>
                                    {
                                        t.AlignRight();
                                        if (bold)
                                            t.Span(valor ?? "").FontColor("#233a7b").Bold();
                                        else
                                            t.Span(valor ?? "").FontColor("#444");
                                    });
                            }

                            AddRow("Salario Base", FormatoMoneda(model.SalarioBase, model.Moneda));
                            AddRow("Horas Extras", model.HorasExtras.ToString("N2", new CultureInfo("es-GT")));
                            AddRow("Bonificaciones", FormatoMoneda(model.Bonificaciones, model.Moneda));
                            AddRow("Descuentos", FormatoMoneda(model.Descuentos, model.Moneda));
                            AddRow("IGSS", FormatoMoneda(model.IGSS, model.Moneda));
                            AddRow("IRTRA", FormatoMoneda(model.IRTRA, model.Moneda));
                            AddRow("ISR", FormatoMoneda(model.ISR, model.Moneda));
                            AddRow("Total a Pagar", FormatoMoneda(model.TotalPagar, model.Moneda), bold: true);
                        });
                    });

                    // FOOTER
                    page.Footer()
                        .PaddingTop(20)
                        .AlignRight()
                        .Text(t =>
                        {
                            t.Span("Generado el ");
                            t.Span(DateTime.Now.ToString("dd/MM/yyyy")).SemiBold().FontSize(10).FontColor("#666");
                        });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        private byte[] GenerarPdfNominaCompleta(Nomina nomina)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                    page.Header().Text($"Nómina Completa - {nomina.Periodo}")
                        .FontColor("#233a7b").SemiBold().FontSize(18);

                    page.Content().Element(c =>
                    {
                        c.Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2); // Empleado
                                columns.RelativeColumn();   // Salario Base
                                columns.RelativeColumn();   // Horas Extras
                                columns.RelativeColumn();   // Bonificaciones
                                columns.RelativeColumn();   // Descuentos
                                columns.RelativeColumn();   // IGSS
                                columns.RelativeColumn();   // IRTRA
                                columns.RelativeColumn();   // ISR
                                columns.RelativeColumn();   // TotalPagar
                            });

                            // Header
                            table.Cell().ColumnSpan(9).Element(CellStyle).Text($"Periodo: {nomina.FechaInicio:dd/MM/yyyy} - {nomina.FechaFin:dd/MM/yyyy}").Bold();
                            table.Cell().Text("Empleado").Bold();
                            table.Cell().Text("Salario Base").Bold();
                            table.Cell().Text("Horas Extras").Bold();
                            table.Cell().Text("Bonificaciones").Bold();
                            table.Cell().Text("Descuentos").Bold();
                            table.Cell().Text("IGSS").Bold();
                            table.Cell().Text("IRTRA").Bold();
                            table.Cell().Text("ISR").Bold();
                            table.Cell().Text("Total Pagar").Bold();

                            // Body
                            foreach (var d in nomina.Detalles)
                            {
                                table.Cell().Text($"{d.Empleado?.Nombre} {d.Empleado?.Apellido}");
                                table.Cell().Text(d.SalarioBase.ToString("N2"));
                                table.Cell().Text(d.HorasExtras.ToString("N2"));
                                table.Cell().Text(d.Bonificaciones.ToString("N2"));
                                table.Cell().Text(d.Descuentos.ToString("N2"));
                                table.Cell().Text(d.IGSS.ToString("N2"));
                                table.Cell().Text(d.IRTRA.ToString("N2"));
                                table.Cell().Text(d.ISR.ToString("N2"));
                                table.Cell().Text(d.TotalPagar.ToString("N2"));
                            }
                        });
                    });
                    page.Footer()
                        .PaddingTop(20)
                        .AlignRight()
                        .Text($"Generado el {DateTime.Now:dd/MM/yyyy}");
                });
            }).GeneratePdf();

            return pdfBytes;

            IContainer CellStyle(IContainer container) => container.Padding(5);
        }

        private string FormatoMoneda(decimal valor, string moneda)
        {
            if (moneda == "GTQ")
                return valor.ToString("N2", new CultureInfo("es-GT")) + " GTQ";
            return valor.ToString("C2", CultureInfo.CurrentCulture);
        }
    }
}