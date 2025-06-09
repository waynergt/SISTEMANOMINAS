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
        {
            try
            {
                return Ok(await _service.ValidarExpediente(empleadoId));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message + " - " + ex.StackTrace);
            }
        }
        [HttpGet("tipos-obligatorios")]
        public async Task<ActionResult<List<string>>> GetTiposDocumentoObligatorios()
        {
            var tipos = await _context.ConfiguracionesExpediente
                .Where(c => c.Obligatorio)
                .Select(c => c.TipoDocumento)
                .ToListAsync();
            return Ok(tipos);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarDocumento(int id)
        {
            var doc = await _context.DocumentosEmpleado.FindAsync(id);
            if (doc == null)
                return NotFound();

            // Ruta física
            var rutaFisica = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.RutaArchivo.TrimStart('/'));

            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }

            _context.DocumentosEmpleado.Remove(doc);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPut("editar")]
        public async Task<IActionResult> EditarDocumento([FromForm] EditarDocumentoExpedienteDto dto)
        {
            // Busca el documento original
            var doc = await _context.DocumentosEmpleado.FindAsync(dto.Id);
            if (doc == null)
                return NotFound("Documento no encontrado.");

            // Actualiza el tipo de documento si cambió
            if (!string.IsNullOrWhiteSpace(dto.TipoDocumento))
                doc.TipoDocumento = dto.TipoDocumento;

            // Si se subió un archivo, reemplaza el archivo físico y la ruta
            if (dto.Archivo != null)
            {
                // Elimina el archivo anterior si existe
                var rutaFisicaAnterior = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.RutaArchivo.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisicaAnterior))
                    System.IO.File.Delete(rutaFisicaAnterior);

                // Guardar el nuevo archivo
                var nombreNuevo = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
                var rutaRelativa = $"/uploads/{nombreNuevo}";
                var rutaFisicaNueva = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", nombreNuevo);

                Directory.CreateDirectory(Path.GetDirectoryName(rutaFisicaNueva)!);

                using (var stream = new FileStream(rutaFisicaNueva, FileMode.Create))
                {
                    await dto.Archivo.CopyToAsync(stream);
                }

                doc.RutaArchivo = rutaRelativa;
                doc.FechaSubida = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}