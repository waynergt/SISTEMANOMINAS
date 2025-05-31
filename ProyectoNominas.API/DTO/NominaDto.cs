using System;
using System.ComponentModel.DataAnnotations;

public class NominaDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un empleado")]
    public int EmpleadoId { get; set; }

    [Required(ErrorMessage = "Debe ingresar la fecha de pago")]
    public DateTime? FechaPago { get; set; }

    [Required(ErrorMessage = "Debe ingresar el monto total")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal? MontoTotal { get; set; }

    public string? NombreEmpleado { get; set; }
}