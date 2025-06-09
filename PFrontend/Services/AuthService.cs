using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using PFrontend.Models;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PFrontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly IJSRuntime _js;

        public AuthService(HttpClient http, IJSRuntime js)
        {
            _http = http;
            _js = js;
        }

        public async Task<(bool ok, string? error)> LoginAsync(string correo, string contrasena)
        {
            var req = new LoginRequest { Correo = correo, Contrasena = contrasena };
            HttpResponseMessage resp;
            try
            {
                resp = await _http.PostAsJsonAsync("api/auth/login", req);
            }
            catch
            {
                return (false, "No se pudo conectar al servidor.");
            }

            if (resp.IsSuccessStatusCode)
            {
                var result = await resp.Content.ReadFromJsonAsync<LoginResult>();
                if (result?.token != null)
                {
                    await _js.InvokeVoidAsync("localStorage.setItem", "authToken", result.token);
                    return (true, null);
                }
                return (false, "Respuesta inválida del servidor.");
            }
            else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                // Mensaje amigable para credenciales incorrectas
                return (false, "Correo o contraseña incorrectos");
            }
            else
            {
                return (false, "Ocurrió un error al intentar iniciar sesión.");
            }
        }

        public async Task LogoutAsync()
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", "authToken");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token)) return false;

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jwt = null;
            try
            {
                jwt = handler.ReadJwtToken(token);
            }
            catch
            {
                return false;
            }

            var exp = jwt.ValidTo;
            return exp > DateTime.UtcNow;
        }

        public async Task<ClaimsPrincipal?> GetUserAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "authToken");
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jwt = handler.ReadJwtToken(token);
                var claims = jwt.Claims;
                var identity = new ClaimsIdentity(claims, "jwt");
                return new ClaimsPrincipal(identity);
            }
            catch
            {
                return null;
            }
        }

        private class LoginResult
        {
            public string? token { get; set; }
        }
    }
}