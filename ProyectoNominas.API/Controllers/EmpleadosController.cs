using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmpleadosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmpleadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Empleados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmpleadoDto>>> GetEmpleados()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.Puesto)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    NombreCompleto = $"{e.Nombre} {e.Apellido}",
                    Apellido = e.Apellido,
                    Dpi = e.Dpi,
                    Correo = e.Correo,
                    Salario = e.Salario,
                    DepartamentoId = e.DepartamentoId,
                    PuestoId = e.PuestoId,
                    Departamento = e.Departamento != null ? e.Departamento.Nombre : string.Empty,
                    Puesto = e.Puesto != null ? e.Puesto.Nombre : string.Empty
                })
                .ToListAsync();

            return Ok(empleados);
        }

        // GET: api/Empleados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmpleadoDto>> GetEmpleado(int id)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.Puesto)
                .Where(e => e.Id == id)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    NombreCompleto = $"{e.Nombre} {e.Apellido}",
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Dpi = e.Dpi,
                    Correo = e.Correo,
                    Salario = e.Salario,
                    DepartamentoId = e.DepartamentoId,
                    PuestoId = e.PuestoId,
                    Departamento = e.Departamento != null ? e.Departamento.Nombre : string.Empty,
                    Puesto = e.Puesto != null ? e.Puesto.Nombre : string.Empty
                })
                .FirstOrDefaultAsync();

            if (empleado == null)
                return NotFound();

            return Ok(empleado);
        }

        // GET: api/Empleados/dpi/9988776655443
        [HttpGet("dpi/{dpi}")]
        public async Task<ActionResult<EmpleadoDto>> GetEmpleadoPorDpi(string dpi)
        {
            var empleado = await _context.Empleados
                .Include(e => e.Departamento)
                .Include(e => e.Puesto)
                .Where(e => e.Dpi == dpi)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    NombreCompleto = $"{e.Nombre} {e.Apellido}",
                    Nombre = e.Nombre,
                    Apellido = e.Apellido,
                    Dpi = e.Dpi,
                    Correo = e.Correo,
                    Salario = e.Salario,
                    DepartamentoId = e.DepartamentoId,
                    PuestoId = e.PuestoId,
                    Departamento = e.Departamento != null ? e.Departamento.Nombre : string.Empty,
                    Puesto = e.Puesto != null ? e.Puesto.Nombre : string.Empty
                })
                .FirstOrDefaultAsync();

            if (empleado == null)
                return NotFound();

            return Ok(empleado);
        }

        // POST: api/Empleados
        [HttpPost]
        public async Task<ActionResult<Empleado>> PostEmpleado(Empleado empleado)
        {
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
        }

        // PUT: api/Empleados/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, Empleado empleado)
        {
            if (id != empleado.Id)
                return BadRequest();

            _context.Entry(empleado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Empleados.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Empleados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmpleado(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            _context.Empleados.Remove(empleado);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}