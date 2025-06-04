using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTO;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PuestoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PuestoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Puesto
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PuestoDto>>> GetPuestos()
        {
            return await _context.Puestos
                .Select(p => new PuestoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Activo = p.Activo
                })
                .ToListAsync();
        }

        // GET: api/Puesto/activos
        [HttpGet("activos")]
        public async Task<ActionResult<IEnumerable<PuestoDto>>> GetPuestosActivos()
        {
            return await _context.Puestos
                .Where(p => p.Activo)
                .Select(p => new PuestoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Activo = p.Activo
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PuestoDto>> GetPuesto(int id)
        {
            var puesto = await _context.Puestos.FindAsync(id);
            if (puesto == null)
                return NotFound();

            return new PuestoDto
            {
                Id = puesto.Id,
                Nombre = puesto.Nombre,
                Activo = puesto.Activo
            };
        }

        [HttpPost]
        public async Task<ActionResult<PuestoDto>> PostPuesto(PuestoDto puestoDto)
        {
            var puesto = new Puesto
            {
                Nombre = puestoDto.Nombre,
                Activo = puestoDto.Activo
            };
            _context.Puestos.Add(puesto);
            await _context.SaveChangesAsync();

            puestoDto.Id = puesto.Id;
            return CreatedAtAction(nameof(GetPuesto), new { id = puesto.Id }, puestoDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutPuesto(int id, PuestoDto puestoDto)
        {
            if (id != puestoDto.Id)
                return BadRequest();

            var puesto = await _context.Puestos.FindAsync(id);
            if (puesto == null)
                return NotFound();

            puesto.Nombre = puestoDto.Nombre;
            puesto.Activo = puestoDto.Activo;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePuesto(int id)
        {
            var puesto = await _context.Puestos.FindAsync(id);
            if (puesto == null)
                return NotFound();

            _context.Puestos.Remove(puesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}