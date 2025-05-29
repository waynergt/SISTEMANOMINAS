namespace ProyectoNominas.API.Controllers
{
    public class EmpleadoEditDTO
    {
        internal readonly string? Nombre;
        internal readonly string? Apellido;
        internal readonly string? Correo;
        internal readonly string? Dpi;
        internal readonly int DepartamentoId;
        internal readonly decimal Salario;
        internal readonly int PuestoId;

        public int Id { get; internal set; }
    }
}