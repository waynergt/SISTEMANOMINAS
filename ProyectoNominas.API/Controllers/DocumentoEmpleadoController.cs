using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentoEmpleadoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DocumentoEmpleadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DocumentoEmpleado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoEmpleado>>> GetDocumentos()
        {
            return await _context.DocumentosEmpleado
                .Include(d => d.Empleado)
                .ToListAsync();
        }

        // GET: api/DocumentoEmpleado/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoEmpleado>> GetDocumento(int id)
        {
            var documento = await _context.DocumentosEmpleado
                .Include(d => d.Empleado)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
                return NotFound();

            return documento;
        }

        // POST: api/DocumentoEmpleado
        [HttpPost]
        public async Task<ActionResult<DocumentoEmpleado>> PostDocumento(DocumentoEmpleado documento)
        {
            _context.DocumentosEmpleado.Add(documento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDocumento), new { id = documento.Id }, documento);
        }

        // PUT: api/DocumentoEmpleado/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDocumento(int id, DocumentoEmpleado documento)
        {
            if (id != documento.Id)
                return BadRequest();

            _context.Entry(documento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.DocumentosEmpleado.Any(d => d.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/DocumentoEmpleado/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDocumento(int id)
        {
            var documento = await _context.DocumentosEmpleado.FindAsync(id);
            if (documento == null)
                return NotFound();

            _context.DocumentosEmpleado.Remove(documento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
