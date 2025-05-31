using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;

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
        public async Task<ActionResult<IEnumerable<Puesto>>> GetPuestos()
        {
            return await _context.Puestos.ToListAsync();
        }

        // GET: api/Puesto/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Puesto>> GetPuesto(int id)
        {
            var puesto = await _context.Puestos.FindAsync(id);

            if (puesto == null)
                return NotFound();

            return puesto;
        }

        // POST: api/Puesto
        [HttpPost]
        public async Task<ActionResult<Puesto>> PostPuesto([FromBody] Puesto puesto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Puestos.Add(puesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPuesto), new { id = puesto.Id }, puesto);
        }

        // PUT: api/Puesto/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPuesto(int id, [FromBody] Puesto puesto)
        {
            if (id != puesto.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(puesto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Puestos.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Puesto/5
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