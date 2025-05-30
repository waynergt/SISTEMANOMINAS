public class HistorialAjusteNomina
{
    public int Id { get; set; }
    public int DetalleNominaId { get; set; }
    public DetalleNomina? DetalleNomina { get; set; }
    public DateTime FechaAjuste { get; set; }
    public string Concepto { get; set; } = "";
    public decimal ValorAnterior { get; set; }
    public decimal ValorNuevo { get; set; }
    public string UsuarioAjuste { get; set; } = "";
    public string Motivo { get; set; } = "";
}