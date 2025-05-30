using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Services;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ReporteNominaService _reporteService;

        public ReporteController(ApplicationDbContext context, ReporteNominaService reporteService)
        {
            _context = context;
            _reporteService = reporteService;
        }

        // Empleados por estado
        [HttpGet("empleados")]
        public async Task<IActionResult> GenerarReporteEmpleados([FromQuery] string estado)
        {
            var empleados = await _context.Empleados
                .Where(e => e.EstadoLaboral.ToLower() == estado.ToLower())
                .ToListAsync();

            if (!empleados.Any())
                return NotFound($"No se encontraron empleados con estado '{estado}'.");

            var pdf = _reporteService.GenerarReporteEmpleados(empleados, estado);

            return File(pdf, "application/pdf", $"reporte_empleados_{estado.ToLower()}.pdf");
        }

        // Expediente empleado por ID
        [HttpGet("expediente")]
        public async Task<IActionResult> GenerarReporteExpedienteEmpleado([FromQuery] int empleadoId)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Documentos)
                .Include(e => e.Estudios)
                .Include(e => e.Nominas).ThenInclude(n => n.Detalles)
                .FirstOrDefaultAsync(e => e.Id == empleadoId);

            if (empleado == null)
                return NotFound("Empleado no encontrado.");

            var pdf = _reporteService.GenerarReporteExpedienteEmpleado(empleado);

            return File(pdf, "application/pdf", $"expediente_empleado_{empleadoId}.pdf");
        }

        // Descuentos por DPI (usando DetalleDescuentoNomina)
        [HttpGet("descuentos-dpi")]
        public async Task<IActionResult> ReporteDescuentosPorDpi([FromQuery] string dpi)
        {
            var detalles = await _context.DetallesDescuentoNomina
                .Include(d => d.Nomina).ThenInclude(n => n.Detalles)
                .Include(d => d.DescuentoLegal)
                .Where(d => d.Nomina.Detalles.Any(x => x.Empleado.Dpi == dpi))
                .ToListAsync();

            if (!detalles.Any())
                return NotFound("No se encontraron descuentos para ese DPI.");

            var pdf = _reporteService.GenerarReporteDescuentosPorDpi(detalles, dpi);
            return File(pdf, "application/pdf", $"reporte_descuentos_{dpi}.pdf");
        }

        // GET: api/Reporte/nomina-por-periodo?inicio=2024-01-01&fin=2024-02-01
        [HttpGet("nomina-por-periodo")]
        public async Task<IActionResult> GenerarReporteNominaPorPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var nominas = await _context.Nominas
                .Include(n => n.Detalles).ThenInclude(d => d.Empleado)
                .Where(n => n.FechaInicio >= inicio && n.FechaFin <= fin)
                .ToListAsync();

            if (!nominas.Any())
                return NotFound("No hay nóminas en el período seleccionado.");

            var pdf = _reporteService.GenerarReporteNominasPorPeriodo(nominas, inicio, fin);
            return File(pdf, "application/pdf", $"reporte_nominas_{inicio:yyyyMMdd}_{fin:yyyyMMdd}.pdf");
        }

        // GET: api/Reporte/expediente-dpi?dpi=XXXXXXXXXXXX
        [HttpGet("expediente-dpi")]
        public async Task<IActionResult> GenerarReporteExpedientePorDpi([FromQuery] string dpi)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Documentos)
                    .Include(e => e.Estudios)
                    .Include(e => e.Nominas).ThenInclude(n => n.Detalles)
                    .FirstOrDefaultAsync(e => e.Dpi == dpi);

                if (empleado == null)
                    return NotFound("Empleado no encontrado.");

                var pdf = _reporteService.GenerarReporteExpedienteEmpleado(empleado);
                return File(pdf, "application/pdf", $"expediente_{dpi}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}