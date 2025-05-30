using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTOs;
using ProyectoNominas.API.Services;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpedienteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ExpedienteService _service;

        public ExpedienteController(ApplicationDbContext context, ExpedienteService service)
        {
            _context = context;
            _service = service;
        }

        // GET: api/Expediente/validar/1 (VALIDACIÓN usando _context directamente)
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

        // GET: api/Expediente/empleado/{empleadoId}
        [HttpGet("empleado/{empleadoId}")]
        public async Task<ActionResult<List<DocumentoExpedienteDto>>> GetDocsEmpleado(int empleadoId)
            => Ok(await _service.ObtenerDocumentosPorEmpleado(empleadoId));

        // POST: api/Expediente/upload
        [HttpPost("upload")]
        public async Task<ActionResult> Upload([FromForm] CrearDocumentoExpedienteDto dto)
        {
            var doc = await _service.SubirDocumento(dto);
            if (doc == null) return BadRequest("Error al subir el documento.");
            return Ok(doc);
        }

        // GET: api/Expediente/validar2/{empleadoId} (VALIDACIÓN usando el service y DTO)
        [HttpGet("validar2/{empleadoId}")]
        public async Task<ActionResult<ExpedienteValidacionDto>> ValidarExpedienteDto(int empleadoId)
            => Ok(await _service.ValidarExpediente(empleadoId));
    }
}