namespace ProyectoNominas.API.DTOs
{
    public class GenerarNominaRequest
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Periodo { get; set; } = "";
    public int? EmpleadoId { get; set; } // ← este es nuevo
}
}