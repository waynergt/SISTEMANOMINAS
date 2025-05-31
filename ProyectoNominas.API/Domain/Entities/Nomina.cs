public class Nomina
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Periodo { get; set; } = ""; // Ejemplo: "2025-05-Q1"
    public decimal MontoTotal { get; set; }
    public ICollection<DetalleNomina> Detalles { get; set; } = new List<DetalleNomina>();
}