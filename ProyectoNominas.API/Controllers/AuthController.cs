using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public class LoginRequest
        {
            public string Correo { get; set; } = string.Empty;
            public string Contrasena { get; set; } = string.Empty;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u =>
                u.Correo == request.Correo && u.ContrasenaHash == request.Contrasena && u.EstaActivo);

            if (usuario == null)
                return Unauthorized("Credenciales inválidas");

            var roles = _context.UsuarioRoles
                .Where(ur => ur.UsuarioId == usuario.Id)
                .Select(ur => ur.Rol!.Nombre)
                .ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Email, usuario.Correo)
            };

            foreach (var rol in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, rol));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { token = jwt });
        }
    }
}
