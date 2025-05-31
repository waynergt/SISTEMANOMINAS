namespace ProyectoNominas.API.DTO
{
    public class EmpleadoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Dpi { get; set; } = "";
        public decimal Salario { get; set; }
        public string EstadoLaboral { get; set; } = "Activo";
        public int DepartamentoId { get; set; }
        public string? Departamento { get; set; }
        public int PuestoId { get; set; }
        public string? Puesto { get; set; }
    }
}