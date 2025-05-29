using System;
using System.Collections.Generic;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Dpi { get; set; } = string.Empty;
        public decimal Salario { get; set; }
        public string EstadoLaboral { get; set; } = "Activo";

        public int DepartamentoId { get; set; }
        public Departamento? Departamento { get; set; }

        public int PuestoId { get; set; }
        public Puesto? Puesto { get; set; }

        public ICollection<DocumentoEmpleado>? Documentos { get; set; }
        public ICollection<InformacionAcademica>? Estudios { get; set; }
        public ICollection<Nomina>? Nominas { get; set; }
    }
}
