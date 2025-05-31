namespace ProyectoNominas.API.DTOs
{
    public class DocumentoExpedienteDto
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public string RutaArchivo { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; }
    }
}