namespace ProyectoNominas.API.Domain.Entities
{
    public class DetalleDescuentoNomina
    {
        public int Id { get; set; }

        public int NominaId { get; set; }
        public Nomina? Nomina { get; set; }

        public int DescuentoLegalId { get; set; }
        public DescuentoLegal? DescuentoLegal { get; set; }

        public decimal Monto { get; set; }
    }
}
