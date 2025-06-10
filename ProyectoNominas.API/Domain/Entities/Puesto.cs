using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Puesto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del puesto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        // Relación con empleados
        public ICollection<Empleado>? Empleados { get; set; }
    }
}