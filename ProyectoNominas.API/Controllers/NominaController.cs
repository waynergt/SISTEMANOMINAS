using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTO;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NominaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NominaDto>>> GetNominas()
        {
            var nominas = await _context.Nominas
                .Include(n => n.Empleado)
                .Select(n => new NominaDto
                {
                    Id = n.Id,
                    FechaPago = n.FechaPago,
                    MontoTotal = n.MontoTotal,
                    EmpleadoId = n.EmpleadoId,
                    NombreEmpleado = n.Empleado != null ? $"{n.Empleado.Nombre} {n.Empleado.Apellido}" : ""
                })
                .ToListAsync();

            return Ok(nominas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NominaDto>> GetNomina(int id)
        {
            var nomina = await _context.Nominas
                .Include(n => n.Empleado)
                .Where(n => n.Id == id)
                .Select(n => new NominaDto
                {
                    Id = n.Id,
                    FechaPago = n.FechaPago,
                    MontoTotal = n.MontoTotal,
                    EmpleadoId = n.EmpleadoId,
                    NombreEmpleado = n.Empleado != null ? $"{n.Empleado.Nombre} {n.Empleado.Apellido}" : ""
                })
                .FirstOrDefaultAsync();

            if (nomina == null)
                return NotFound();

            return Ok(nomina);
        }

        [HttpPost]
        public async Task<ActionResult> PostNomina(NominaDto dto)
        {
            if (dto.FechaPago == null)
            {
                return BadRequest("La fecha de pago no puede ser nula.");
            }

            var nomina = new Nomina
            {
                FechaPago = dto.FechaPago.Value, // Explicitly convert nullable DateTime to non-nullable
                MontoTotal = dto.MontoTotal ?? 0, // Provide a default value for nullable decimal
                EmpleadoId = dto.EmpleadoId
            };

            _context.Nominas.Add(nomina);
            await _context.SaveChangesAsync();
            dto.Id = nomina.Id;
            return CreatedAtAction(nameof(GetNomina), new { id = nomina.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutNomina(int id, NominaDto dto)
        {
            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null) return NotFound();

            if (dto.FechaPago == null)
            {
                return BadRequest("La fecha de pago no puede ser nula.");
            }

            nomina.FechaPago = dto.FechaPago.Value; // Explicitly convert nullable DateTime to non-nullable
            nomina.MontoTotal = dto.MontoTotal ?? 0; // Provide a default value for nullable decimal
            nomina.EmpleadoId = dto.EmpleadoId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNomina(int id)
        {
            var nomina = await _context.Nominas.FindAsync(id);
            if (nomina == null) return NotFound();

            _context.Nominas.Remove(nomina);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}