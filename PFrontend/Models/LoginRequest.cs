using System.ComponentModel.DataAnnotations;

namespace PFrontend.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Correo es obligatorio")]
        public string Correo { get; set; } 

        [Required(ErrorMessage = "Contraseña es obligatoria")]
        public string Contrasena { get; set; } 
    }
}