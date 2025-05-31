namespace ProyectoNominas.API.DTO
{
    public class EmpleadoEditDto
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Correo { get; set; } = "";
        public string Dpi { get; set; } = "";
        public decimal Salario { get; set; }
        public string EstadoLaboral { get; set; } 
        public int DepartamentoId { get; set; }
        public int PuestoId { get; set; }
    }
}