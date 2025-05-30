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

        // GET: api/Reporte/empleados?estado=Activo
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

        // GET: api/Reporte/expediente?empleadoId=1
        [HttpGet("expediente")]
        public async Task<IActionResult> GenerarReporteExpedienteEmpleado([FromQuery] int empleadoId)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Documentos)
                .Include(e => e.Estudios)
                .Include(e => e.Nominas)
                .FirstOrDefaultAsync(e => e.Id == empleadoId);

            if (empleado == null)
                return NotFound("Empleado no encontrado.");

            var pdf = _reporteService.GenerarReporteExpedienteEmpleado(empleado);

            return File(pdf, "application/pdf", $"expediente_empleado_{empleadoId}.pdf");
        }

    }
}
