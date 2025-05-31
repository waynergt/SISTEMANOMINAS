namespace ProyectoNominas.API.DTOs
{
    public class ExpedienteValidacionDto
    {
        public bool ExpedienteCompleto { get; set; }
        public List<string> DocumentosFaltantes { get; set; } = new();
    }
}