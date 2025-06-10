using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.DTOs;
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            Console.WriteLine($"Correo recibido: {request.Correo}");
            Console.WriteLine($"Contraseña recibida: {request.Contrasena}");

            var usuario = await _context.Usuarios
                .Include(u => u.UsuarioRoles)
                    .ThenInclude(ur => ur.Rol)
                .FirstOrDefaultAsync(u => u.Correo == request.Correo);

            if (usuario == null)
            {
                Console.WriteLine("Usuario NO encontrado");
                return Unauthorized();
            }

            Console.WriteLine($"Hash en BD: {usuario.ContrasenaHash}");

            if (!BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.ContrasenaHash))
            {
                Console.WriteLine("Contraseña incorrecta");
                return Unauthorized();
            }

            Console.WriteLine("Login exitoso");

            var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var jwtKey = _config["Jwt:Key"];
            if (jwtKey == null)
                return StatusCode(500, "JWT Key no configurada en appsettings.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new
            {
                token = tokenString,
                usuario = new
                {
                    usuario.Id,
                    usuario.Correo,
                    Roles = roles
                }
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegistroDto dto)
        {
            // Validación básica
            if (string.IsNullOrEmpty(dto.NombreUsuario) || string.IsNullOrEmpty(dto.Correo) || string.IsNullOrEmpty(dto.Contrasena))
                return BadRequest("Todos los campos son obligatorios.");

            // Verificar si el correo ya existe
            if (await _context.Usuarios.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("Ya existe un usuario con este correo.");

            // Hashear la contraseña
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Contrasena);

            // Crear el usuario
            var nuevoUsuario = new ProyectoNominas.API.Domain.Entities.Usuario
            {
                NombreUsuario = dto.NombreUsuario,
                Correo = dto.Correo,
                ContrasenaHash = passwordHash,
                EstaActivo = dto.EstaActivo
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            // Asignar roles si se envían (opcional)
            if (dto.RolesId != null && dto.RolesId.Count > 0)
            {
                foreach (var rolId in dto.RolesId)
                {
                    _context.UsuarioRoles.Add(new ProyectoNominas.API.Domain.Entities.UsuarioRol
                    {
                        UsuarioId = nuevoUsuario.Id,
                        RolId = rolId
                    });
                }
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Usuario registrado correctamente" });
        }
    }
}