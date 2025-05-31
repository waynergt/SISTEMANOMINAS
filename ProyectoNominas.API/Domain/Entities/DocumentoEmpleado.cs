
namespace ProyectoNominas.API.Domain.Entities
{
    public class DocumentoEmpleado
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;

        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }
        public DateTime FechaSubida { get; internal set; }
    }
}