using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProyectoNominas.API.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProyectoNominas.API.Services
{
    public class ReporteNominaService
    {
        // Reporte de Empleados por estado
        public byte[] GenerarReporteEmpleados(List<Empleado> empleados, string estado)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Empleados con estado: {estado}").FontSize(18).Bold().AlignCenter();

                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Element(CellEstilo).Text("Nombre");
                            header.Cell().Element(CellEstilo).Text("Apellido");
                            header.Cell().Element(CellEstilo).Text("DPI");
                            header.Cell().Element(CellEstilo).Text("Correo");
                        });

                        foreach (var e in empleados)
                        {
                            tabla.Cell().Element(CellEstilo).Text(e.Nombre);
                            tabla.Cell().Element(CellEstilo).Text(e.Apellido);
                            tabla.Cell().Element(CellEstilo).Text(e.Dpi);
                            tabla.Cell().Element(CellEstilo).Text(e.Correo);
                        }
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // Expediente completo de empleado
        public byte[] GenerarReporteExpedienteEmpleado(Empleado empleado)
        {
            var documentos = empleado.Documentos ?? new List<DocumentoEmpleado>();
            var estudios = empleado.Estudios ?? new List<InformacionAcademica>();
            var detallesNomina = empleado.Nominas?.SelectMany(n => n.Detalles ?? new List<DetalleNomina>())
                .Where(d => d.EmpleadoId == empleado.Id)
                .ToList() ?? new List<DetalleNomina>();

            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Expediente completo - {empleado.Nombre} {empleado.Apellido}")
                        .FontSize(20).Bold().AlignCenter();

                    page.Content().Column(column =>
                    {
                        // DATOS PERSONALES
                        column.Item().PaddingBottom(10).Text("👤 Datos personales").Bold().FontSize(14);
                        column.Item().Text($"DPI: {empleado.Dpi}");
                        column.Item().Text($"Correo: {empleado.Correo}");
                        column.Item().Text($"Teléfono: {empleado.Telefono}");
                        column.Item().Text($"Dirección: {empleado.Direccion}");
                        column.Item().Text($"Salario: Q{empleado.Salario:N2}");
                        column.Item().Text($"Estado laboral: {empleado.EstadoLaboral}");

                        // DOCUMENTOS
                        column.Item().PaddingTop(20).Text("📄 Documentos de expediente").Bold().FontSize(14);
                        if (documentos.Any())
                        {
                            column.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Element(CellEstilo).Text("Tipo de documento");
                                    header.Cell().Element(CellEstilo).Text("Ruta");
                                });

                                foreach (var doc in documentos)
                                {
                                    tabla.Cell().Element(CellEstilo).Text(doc.TipoDocumento);
                                    tabla.Cell().Element(CellEstilo).Text(doc.RutaArchivo);
                                }
                            });
                        }
                        else
                        {
                            column.Item().Text("No hay documentos cargados.");
                        }

                        // INFORMACIÓN ACADÉMICA
                        column.Item().PaddingTop(20).Text("🎓 Información académica").Bold().FontSize(14);
                        if (estudios.Any())
                        {
                            column.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Element(CellEstilo).Text("Título");
                                    header.Cell().Element(CellEstilo).Text("Institución");
                                    header.Cell().Element(CellEstilo).Text("Fecha de graduación");
                                });

                                foreach (var est in estudios)
                                {
                                    tabla.Cell().Element(CellEstilo).Text(est.Titulo);
                                    tabla.Cell().Element(CellEstilo).Text(est.Institucion);
                                    tabla.Cell().Element(CellEstilo).Text(est.FechaGraduacion.ToShortDateString());
                                }
                            });
                        }
                        else
                        {
                            column.Item().Text("No hay historial académico registrado.");
                        }

                        // HISTORIAL DE NÓMINAS (nuevo: por detalle)
                        column.Item().PaddingTop(20).Text("💵 Historial de nóminas").Bold().FontSize(14);
                        if (detallesNomina.Any())
                        {
                            column.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Periodo
                                    columns.RelativeColumn(); // Fecha inicio
                                    columns.RelativeColumn(); // Fecha fin
                                    columns.RelativeColumn(); // Total a pagar
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Element(CellEstilo).Text("Periodo");
                                    header.Cell().Element(CellEstilo).Text("Fecha Inicio");
                                    header.Cell().Element(CellEstilo).Text("Fecha Fin");
                                    header.Cell().Element(CellEstilo).Text("Total pagado");
                                });

                                foreach (var d in detallesNomina)
                                {
                                    tabla.Cell().Element(CellEstilo).Text(d.Nomina?.Periodo ?? "");
                                    tabla.Cell().Element(CellEstilo).Text(d.Nomina?.FechaInicio.ToShortDateString() ?? "");
                                    tabla.Cell().Element(CellEstilo).Text(d.Nomina?.FechaFin.ToShortDateString() ?? "");
                                    tabla.Cell().Element(CellEstilo).Text($"Q{d.TotalPagar:N2}");
                                }
                            });
                        }
                        else
                        {
                            column.Item().Text("No hay nóminas procesadas.");
                        }
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // Reporte de descuentos por DPI (usando DetalleDescuentoNomina y DescuentoLegal)
        public byte[] GenerarReporteDescuentosPorDpi(List<DetalleDescuentoNomina> detalles, string dpi)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text($"Descuentos por DPI: {dpi}")
                        .FontSize(18).Bold().AlignCenter();

                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Element(CellEstilo).Text("Empleado");
                            header.Cell().Element(CellEstilo).Text("Descuento");
                            header.Cell().Element(CellEstilo).Text("Monto");
                        });

                        foreach (var d in detalles)
                        {
                            var detalleEmpleado = d.Nomina?.Detalles?.FirstOrDefault(x => x.EmpleadoId == d.NominaId)?.Empleado;
                            tabla.Cell().Element(CellEstilo).Text($"{detalleEmpleado?.Nombre ?? "N/A"} {detalleEmpleado?.Apellido ?? ""}");
                            tabla.Cell().Element(CellEstilo).Text(d.DescuentoLegal?.Nombre ?? "N/A");
                            tabla.Cell().Element(CellEstilo).Text($"Q{d.Monto:N2}");
                        }
                    });
                });
            });

            return documento.GeneratePdf();
        }

        // Reporte de nóminas por periodo (detallando empleados)
        public byte[] GenerarReporteNominasPorPeriodo(List<Nomina> nominas, DateTime inicio, DateTime fin)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text($"Nómina del {inicio:dd/MM/yyyy} al {fin:dd/MM/yyyy}").FontSize(18).Bold().AlignCenter();

                    page.Content().Table(tabla =>
                    {
                        tabla.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        tabla.Header(header =>
                        {
                            header.Cell().Element(CellEstilo).Text("Empleado");
                            header.Cell().Element(CellEstilo).Text("Periodo");
                            header.Cell().Element(CellEstilo).Text("Rango Fechas");
                            header.Cell().Element(CellEstilo).Text("Total Pagado");
                        });

                        foreach (var nomina in nominas)
                        {
                            foreach (var detalle in nomina.Detalles)
                            {
                                tabla.Cell().Element(CellEstilo).Text($"{detalle.Empleado?.Nombre} {detalle.Empleado?.Apellido}");
                                tabla.Cell().Element(CellEstilo).Text(nomina.Periodo);
                                tabla.Cell().Element(CellEstilo).Text($"{nomina.FechaInicio:dd/MM/yyyy} - {nomina.FechaFin:dd/MM/yyyy}");
                                tabla.Cell().Element(CellEstilo).Text($"Q{detalle.TotalPagar:N2}");
                            }
                        }
                    });
                });
            });

            return documento.GeneratePdf();
        }

        private IContainer CellEstilo(IContainer container)
        {
            return container
                .Padding(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }
    }
}