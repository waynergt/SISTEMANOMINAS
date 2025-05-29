namespace ProyectoNominas.API.Domain.Entities
{
    public class DescuentoLegal
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<DetalleDescuentoNomina> DetallesDescuento { get; set; } = new List<DetalleDescuentoNomina>();
    }
}
