

using ProyectoNominas.API.Domain.Entities;

public class HorasTrabajadas
{
    public int Id { get; set; }
    public int EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }
    public DateTime Fecha { get; set; }
    public decimal HorasNormales { get; set; }
    public decimal HorasExtras { get; set; }
}