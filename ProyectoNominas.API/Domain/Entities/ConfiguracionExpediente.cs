namespace ProyectoNominas.API.Domain.Entities
{
    public class ConfiguracionExpediente
    {
        public int Id { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public bool Obligatorio { get; set; } = true;
    }
}
