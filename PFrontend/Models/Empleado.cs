namespace PFrontend.Models
{
    namespace PFrontend.Models
    {
        public class Empleado
        {
            public int Id { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public string Apellido { get; set; } = string.Empty;
            public string Dpi { get; set; } = string.Empty;
            public string Correo { get; set; } = string.Empty;
            public decimal Salario { get; set; }
            public int DepartamentoId { get; set; }
            public int PuestoId { get; set; }
        }
    }

    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class Puesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
