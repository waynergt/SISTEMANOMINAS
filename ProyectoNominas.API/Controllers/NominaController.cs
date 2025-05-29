using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.Services;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ReporteNominaService _reporteService;

        public NominaController(ApplicationDbContext context, ReporteNominaService reporteService)
        {
            _context = context;
            _reporteService = reporteService;
        }
        // GET: api/Nomina
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Nomina>>> GetNominas()
        {
            return await _context.Nominas
                .Include(n => n.Empleado)
                .ToListAsync();
        }

        // GET: api/Nomina/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Nomina>> GetNomina(int id)
        {
            var nomina = await _context.Nominas
                .Include(n => n.Empleado)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (nomina == null)
                return NotFound();

            return nomina;
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarReporteNomina([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var nominas = await _context.Nominas
                .Include(n => n.Empleado)
                .Where(n => n.FechaPago.Date >= inicio.Date && n.FechaPago.Date <= fin.Date)
                .ToListAsync();

            if (!nominas.Any())
                return NotFound("No hay registros de nómina en el período indicado.");

            var pdf = _reporteService.GenerarReporte(nominas);

            return File(pdf, "application/pdf", $"reporte_nomina_{inicio:yyyyMMdd}_{fin:yyyyMMdd}.pdf");
        }

        // POST: api/Nomina
        [HttpPost]
        public async Task<ActionResult<Nomina>> PostNomina(Nomina nomina)
        {
            var empleado = await _context.Empleados.FindAsync(nomina.EmpleadoId);
            if (empleado == null)
                return BadRequest("Empleado no encontrado.");

            // Obtener documentos obligatorios
            var documentosObligatorios = await _context.ConfiguracionesExpediente
                .Where(c => c.Obligatorio)
                .Select(c => c.TipoDocumento.ToLower())
                .ToListAsync();

            var documentosEmpleado = await _context.DocumentosEmpleado
                .Where(d => d.EmpleadoId == nomina.EmpleadoId)
                .Select(d => d.TipoDocumento.ToLower())
                .ToListAsync();

            var faltantes = documentosObligatorios
                .Where(doc => !documentosEmpleado.Contains(doc))
                .ToList();

            if (faltantes.Any())
            {
                return BadRequest(new
                {
                    mensaje = "El expediente del empleado está incompleto.",
                    documentosFaltantes = faltantes
                });
            }

            // Si el expediente está completo, se registra la nómina
            nomina.MontoTotal = empleado.Salario;
            nomina.FechaPago = DateTime.Now;

            _context.Nominas.Add(nomina);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNomina), new { id = nomina.Id }, nomina);
        }


        // DELETE: api/Nomina/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNomina(int id)
        {
            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null)
                return NotFound();

            _context.Nominas.Remove(nomina);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
