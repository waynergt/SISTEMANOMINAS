using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpedienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExpedienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Expediente/validar/1
        [HttpGet("validar/{empleadoId}")]
        public async Task<ActionResult<object>> ValidarExpediente(int empleadoId)
        {
            var documentosObligatorios = await _context.ConfiguracionesExpediente
                .Where(c => c.Obligatorio)
                .Select(c => c.TipoDocumento.ToLower())
                .ToListAsync();

            var documentosEmpleado = await _context.DocumentosEmpleado
                .Where(d => d.EmpleadoId == empleadoId)
                .Select(d => d.TipoDocumento.ToLower())
                .ToListAsync();

            var faltantes = documentosObligatorios
                .Where(doc => !documentosEmpleado.Contains(doc))
                .ToList();

            return Ok(new
            {
                empleadoId,
                expedienteCompleto = faltantes.Count == 0,
                documentosFaltantes = faltantes
            });
        }
    }
}
