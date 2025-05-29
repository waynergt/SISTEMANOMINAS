namespace ProyectoNominas.API.Dtos
{
    public class NominaDto
    {
        public int Id { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }
        public int EmpleadoId { get; set; }
        public string? NombreEmpleado { get; set; }
        // Puedes agregar más campos relacionados si lo necesitas
    }
}