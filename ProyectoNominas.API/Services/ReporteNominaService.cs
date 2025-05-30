using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProyectoNominas.API.Domain.Entities;

namespace ProyectoNominas.API.Services
{
    public class ReporteNominaService
    {
        // Reporte de Nómina por período
        public byte[] GenerarReporte(List<Nomina> nominas)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Reporte de Nómina").FontSize(20).Bold().AlignCenter();
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
                            header.Cell().Element(CellEstilo).Text("Fecha Pago");
                            header.Cell().Element(CellEstilo).Text("Salario");
                            header.Cell().Element(CellEstilo).Text("Estado");
                        });

                        foreach (var n in nominas)
                        {
                            tabla.Cell().Element(CellEstilo).Text($"{n.Empleado?.Nombre} {n.Empleado?.Apellido}");
                            tabla.Cell().Element(CellEstilo).Text(n.FechaPago.ToShortDateString());
                            tabla.Cell().Element(CellEstilo).Text($"Q{n.MontoTotal:N2}");
                            tabla.Cell().Element(CellEstilo).Text(n.Empleado?.EstadoLaboral);
                        }
                    });
                });
            });

            return documento.GeneratePdf();
        }

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
                            columns.RelativeColumn(); // Nombre
                            columns.RelativeColumn(); // Apellido
                            columns.RelativeColumn(); // DPI
                            columns.RelativeColumn(); // Correo
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

        // Reporte de Descuentos aplicados a una nómina
        public byte[] GenerarReporteDescuentos(Nomina nomina)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);

                    page.Header().Text($"Descuentos aplicados - Nómina #{nomina.Id}")
                        .FontSize(18).Bold().AlignCenter();

                    page.Content().Column(column =>
                    {
                        column.Item().Text($"Empleado: {nomina.Empleado?.Nombre} {nomina.Empleado?.Apellido}");
                        column.Item().Text($"Fecha de pago: {nomina.FechaPago:dd/MM/yyyy}");
                        column.Item().Text($"Salario bruto: Q{nomina.MontoTotal:N2}");

                        column.Item().PaddingVertical(10).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // Nombre descuento
                                columns.RelativeColumn(); // Monto
                            });

                            tabla.Header(header =>
                            {
                                header.Cell().Element(CellEstilo).Text("Descuento");
                                header.Cell().Element(CellEstilo).Text("Monto");
                            });

                            foreach (var d in nomina.DetallesDescuento!)
                            {
                                tabla.Cell().Element(CellEstilo).Text(d.DescuentoLegal?.Nombre ?? "N/A");
                                tabla.Cell().Element(CellEstilo).Text($"Q{d.Monto:N2}");
                            }
                        });

                        var totalDescuentos = nomina.DetallesDescuento!.Sum(d => d.Monto);
                        var neto = nomina.MontoTotal - totalDescuentos;

                        column.Item().PaddingTop(15).Text($"Total descuentos: Q{totalDescuentos:N2}");
                        column.Item().Text($"Salario neto: Q{neto:N2}").Bold();
                    });

                });
            });

            return documento.GeneratePdf();
        }

        // Reporte de expediente de un empleado
        public byte[] GenerarReporteExpedienteEmpleado(Empleado empleado)
        {
            // Garantiza que las listas no sean nulas
            var documentos = empleado.Documentos ?? new List<DocumentoEmpleado>();
            // Fix for CS0246: Replace 'Estudio' with the correct type 'InformacionAcademica' based on the provided type signatures.
            // Fix for CS0019: Adjust the null-coalescing operator to work with the correct type.

            var estudios = empleado.Estudios?.ToList() ?? new List<InformacionAcademica>();
            var nominas = empleado.Nominas ?? new List<Nomina>();

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
                                    columns.RelativeColumn(); // Título
                                    columns.RelativeColumn(); // Institución
                                    columns.RelativeColumn(); // Fecha
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

                        // HISTORIAL DE NÓMINAS
                        column.Item().PaddingTop(20).Text("💵 Historial de nóminas").Bold().FontSize(14);
                        if (nominas.Any())
                        {
                            column.Item().Table(tabla =>
                            {
                                tabla.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(); // Fecha
                                    columns.RelativeColumn(); // Monto
                                });

                                tabla.Header(header =>
                                {
                                    header.Cell().Element(CellEstilo).Text("Fecha de pago");
                                    header.Cell().Element(CellEstilo).Text("Monto total");
                                });

                                foreach (var n in nominas)
                                {
                                    tabla.Cell().Element(CellEstilo).Text(n.FechaPago.ToShortDateString());
                                    tabla.Cell().Element(CellEstilo).Text($"Q{n.MontoTotal:N2}");
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

        // 🔧 Método reutilizable para estilo de celdas (debe estar dentro de la clase, fuera de los métodos)
        private IContainer CellEstilo(IContainer container)
        {
            return container
                .Padding(5)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }
    }
}