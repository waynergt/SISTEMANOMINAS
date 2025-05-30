using ProyectoNominas.API.Domain.Entities;
using QuestPDF.Fluent;

public class DocumentoEmpleado
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public string? TipoDocumento { get; set; }
    public DateTime FechaSubida { get; set; }
    public string? UrlArchivo { get; set; }

    // Relación de navegación opcional
    public Empleado? Empleado { get; set; }
    public Action<TextDescriptor> RutaArchivo { get; internal set; }
}