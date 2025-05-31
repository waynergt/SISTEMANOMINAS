namespace ProyectoNominas.API.DTO
{
    public class LoginRequest
    {
    }
}
namespace ProyectoNominas.API.Domain.DTOs
{
    public class LoginRequest
    {
        public string Correo { get; set; }
        public string Contrasena { get; set; }
    }
}