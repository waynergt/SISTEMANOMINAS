namespace PFrontend.Models
{
    public class EmpleadoEditModel
    {
        public string Nombre { get; set; } = "";
        public string Apellido { get; set; } = "";
        public string Dpi { get; set; } = "";
        public string Correo { get; set; } = "";
        public decimal Salario { get; set; }
        public string EstadoLaboral { get; set; } = "Activo";
        public int DepartamentoId { get; set; }
        public int PuestoId { get; set; }
    }
}