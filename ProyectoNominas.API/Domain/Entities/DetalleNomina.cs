

using ProyectoNominas.API.Domain.Entities;

public class DetalleNomina
{
    public int Id { get; set; }
    public int NominaId { get; set; }
    public Nomina? Nomina { get; set; }
    public int EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }
    public decimal SalarioBase { get; set; }
    public decimal HorasExtras { get; set; }
    public decimal Bonificaciones { get; set; }
    public decimal Comisiones { get; set; }
    public decimal Descuentos { get; set; }
    public decimal IGSS { get; set; }
    public decimal IRTRA { get; set; }
    public decimal ISR { get; set; }
    public decimal TotalPagar { get; set; }
    public ICollection<HistorialAjusteNomina> AjustesManuales { get; set; } = new List<HistorialAjusteNomina>();
}