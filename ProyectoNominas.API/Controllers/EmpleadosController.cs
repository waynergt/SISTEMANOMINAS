using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.DTO;
// ALIAS para evitar ambigüedad:
using EntidadEmpleado = ProyectoNominas.API.Domain.Entities.Empleado;

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
                    Apellido = e.Apellido,
                    Dpi = e.Dpi,
                    Correo = e.Correo,
                    Salario = e.Salario,
                    DepartamentoId = e.DepartamentoId,
                    PuestoId = e.PuestoId,
                    Departamento = e.Departamento != null ? e.Departamento.Nombre : string.Empty,
                    Puesto = e.Puesto != null ? e.Puesto.Nombre : string.Empty,
                    EstadoLaboral = e.EstadoLaboral
                })
                .ToListAsync();

            return Ok(empleados);
        }

        // GET: api/Empleados/5
         [HttpGet("{id}")]
    public async Task<ActionResult<EmpleadoDetalleDto>> GetEmpleado(int id)
    {
        var empleado = await _context.Empleados
            .Include(e => e.Departamento)
            .Include(e => e.Puesto)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (empleado == null)
            return NotFound();

            var dto = new EmpleadoDetalleDto
            {
                Id = empleado.Id,
                Nombre = empleado.Nombre,
                Apellido = empleado.Apellido,
                FechaNacimiento = empleado.FechaNacimiento,
                Direccion = empleado.Direccion,
                Telefono = empleado.Telefono,
                Correo = empleado.Correo,
                Dpi = empleado.Dpi,
                Salario = empleado.Salario,
                EstadoLaboral = empleado.EstadoLaboral,
                DepartamentoId = empleado.DepartamentoId,
                DepartamentoNombre = empleado.Departamento?.Nombre ?? "",
                PuestoId = empleado.PuestoId,
                PuestoNombre = empleado.Puesto?.Nombre ?? ""
            };

            return Ok(dto);
    }



        // POST: api/Empleados
    
        [HttpPost]
        public async Task<ActionResult<EntidadEmpleado>> PostEmpleado([FromBody] EmpleadoEditDto empleadoDto)
        {
            Console.WriteLine($"Estado recibido: {empleadoDto.EstadoLaboral}"); // O usa un log
            var empleado = new EntidadEmpleado
            {

                Nombre = empleadoDto.Nombre,
                Apellido = empleadoDto.Apellido,
                Dpi = empleadoDto.Dpi,
                Correo = empleadoDto.Correo,
                Salario = empleadoDto.Salario,
                DepartamentoId = empleadoDto.DepartamentoId,
                PuestoId = empleadoDto.PuestoId,
                EstadoLaboral = empleadoDto.EstadoLaboral // <-- AGREGADO
            };

            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmpleado), new { id = empleado.Id }, empleado);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmpleado(int id, [FromBody] EmpleadoEditDto empleadoDto)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
                return NotFound();

            empleado.Nombre = empleadoDto.Nombre;
            empleado.Apellido = empleadoDto.Apellido;
            empleado.Dpi = empleadoDto.Dpi;
            empleado.Correo = empleadoDto.Correo;
            empleado.Salario = empleadoDto.Salario;
            empleado.DepartamentoId = empleadoDto.DepartamentoId;
            empleado.PuestoId = empleadoDto.PuestoId;
            empleado.EstadoLaboral = empleadoDto.EstadoLaboral;

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