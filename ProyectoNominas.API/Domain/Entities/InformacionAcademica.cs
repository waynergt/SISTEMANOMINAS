using System;

namespace ProyectoNominas.API.Domain.Entities
{
    public class InformacionAcademica
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Institucion { get; set; } = string.Empty;
        public DateTime FechaGraduacion { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }
    }
}
