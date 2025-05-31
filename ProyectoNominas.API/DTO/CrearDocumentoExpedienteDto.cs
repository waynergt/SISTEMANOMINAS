using Microsoft.AspNetCore.Http;

namespace ProyectoNominas.API.DTOs
{
    public class CrearDocumentoExpedienteDto
    {
        public int EmpleadoId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public IFormFile Archivo { get; set; } = null!;
    }
}