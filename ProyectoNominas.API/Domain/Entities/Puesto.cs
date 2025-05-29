using System.Collections.Generic;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Puesto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public ICollection<Empleado>? Empleados { get; set; }
    }
}
