using System.Collections.Generic;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Departamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true; // Nuevo campo para estado lógico

        public ICollection<Empleado>? Empleados { get; set; }
    }
}