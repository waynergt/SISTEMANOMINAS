using System.Collections.Generic;

namespace ProyectoNominas.API.Domain.DTOs
{
    public class UsuarioRegistroDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public bool EstaActivo { get; set; } = true;
        public List<int> RolesId { get; set; } = new();
    }
}