using System;

namespace ProyectoNominas.API.Domain.Entities
{
    public class Nomina
    {
        public int Id { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoTotal { get; set; }

        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        // ✅ Agrega esta propiedad si no la tienes:
        public ICollection<DetalleDescuentoNomina>? DetallesDescuento { get; set; }
    }

}
