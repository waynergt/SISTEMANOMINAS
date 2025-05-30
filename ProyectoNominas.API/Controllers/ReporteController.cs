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

        [HttpGet("hay-empleados")]
        public async Task<IActionResult> HayEmpleadosPorEstado([FromQuery] string estado)
        {
            var hay = await _context.Empleados.AnyAsync(e => e.EstadoLaboral.ToLower() == estado.ToLower());
            return Ok(hay);
        }

        [HttpGet("hay-nominas")]
        public async Task<IActionResult> HayNominasPorPeriodo([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var hay = await _context.Nominas.AnyAsync(n => n.FechaPago >= inicio && n.FechaPago <= fin);
            return Ok(hay);
        }

        [HttpGet("hay-descuentos-dpi")]
        public async Task<IActionResult> HayDescuentosPorDpi([FromQuery] string dpi)
        {
            var hay = await _context.Nominas
                .Include(n => n.DetallesDescuento)
                .AnyAsync(n => n.Empleado.Dpi == dpi && n.DetallesDescuento.Any());
            return Ok(hay);
        }

        [HttpGet("hay-expediente-dpi")]
        public async Task<IActionResult> HayExpedienteEmpleadoPorDpi([FromQuery] string dpi)
        {
            var hay = await _context.Empleados.AnyAsync(e => e.Dpi == dpi);
            return Ok(hay);
        }

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

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarReporteNomina([FromQuery] DateTime inicio, [FromQuery] DateTime fin)
        {
            var nominas = await _context.Nominas
                .Include(n => n.Empleado)
                .Where(n => n.FechaPago >= inicio && n.FechaPago <= fin)
                .ToListAsync();

            if (!nominas.Any())
                return NotFound("No hay nóminas en el período seleccionado.");

            var pdf = _reporteService.GenerarReporte(nominas);
            return File(pdf, "application/pdf", $"reporte_nomina_{inicio:yyyyMMdd}_{fin:yyyyMMdd}.pdf");
        }

        [HttpGet("descuentos-dpi")]
        public async Task<IActionResult> GenerarReporteDescuentosPorDpi([FromQuery] string dpi)
        {
            var nomina = await _context.Nominas
                .Include(n => n.Empleado)
                .Include(n => n.DetallesDescuento!)
                    .ThenInclude(d => d.DescuentoLegal)
                .Where(n => n.Empleado.Dpi == dpi)
                .OrderByDescending(n => n.FechaPago)
                .FirstOrDefaultAsync();

            if (nomina == null)
                return NotFound("No se encontraron descuentos para ese DPI.");

            var pdf = _reporteService.GenerarReporteDescuentos(nomina);

            return File(pdf, "application/pdf", $"reporte_descuentos_{dpi}.pdf");
        }

        [HttpGet("expediente-dpi")]
        public async Task<IActionResult> GenerarReporteExpedientePorDpi([FromQuery] string dpi)
        {
            try
            {
                var empleado = await _context.Empleados
                    .Include(e => e.Documentos)
                    .Include(e => e.Estudios)
                    .Include(e => e.Nominas)
                    .FirstOrDefaultAsync(e => e.Dpi == dpi);

                if (empleado == null)
                    return NotFound("Empleado no encontrado.");

                var pdf = _reporteService.GenerarReporteExpedienteEmpleado(empleado);
                return File(pdf, "application/pdf", $"expediente_empleado_{dpi}.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        [HttpGet("estados")]
        public async Task<IActionResult> ObtenerEstados()
        {
            var estados = await _context.Empleados
                .Select(e => e.EstadoLaboral)
                .Distinct()
                .ToListAsync();
            return Ok(estados);
        }
    }
}