using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Apellido { get; set; } = string.Empty;

        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [StringLength(30)]
        public string Telefono { get; set; } = string.Empty;

        [StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Dpi { get; set; } = string.Empty;

        public decimal Salario { get; set; }

        [Required]
        [StringLength(20)]
        public string EstadoLaboral { get; set; } = "Activo";

        // Relaciones
        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public int PuestoId { get; set; }
        public Puesto? Puesto { get; set; }

        public ICollection<DocumentoEmpleado>? Documentos { get; set; }
        public ICollection<InformacionAcademica>? Estudios { get; set; }
        public ICollection<Nomina>? Nominas { get; set; }
    }
}