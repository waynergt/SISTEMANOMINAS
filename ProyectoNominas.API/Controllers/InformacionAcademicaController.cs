using Microsoft.AspNetCore.Mvc;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.Services;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InformacionAcademicaController : ControllerBase
    {
        private readonly InformacionAcademicaService _service;

        public InformacionAcademicaController(InformacionAcademicaService service)
        {
            _service = service;
        }

        // GET: api/InformacionAcademica/empleado/5
        [HttpGet("empleado/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<InformacionAcademica>>> GetPorEmpleado(int empleadoId)
        {
            var items = await _service.ObtenerPorEmpleado(empleadoId);
            return Ok(items);
        }

        // GET: api/InformacionAcademica/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InformacionAcademica>> GetPorId(int id)
        {
            var info = await _service.ObtenerPorId(id);
            if (info == null) return NotFound();
            return Ok(info);
        }

        // POST: api/InformacionAcademica
        [HttpPost]
        public async Task<ActionResult> Crear([FromBody] InformacionAcademica info)
        {
            var ok = await _service.Crear(info);
            if (!ok)
                return BadRequest("Ya existe un título igual para este empleado.");
            return CreatedAtAction(nameof(GetPorId), new { id = info.Id }, info);
        }

        // PUT: api/InformacionAcademica/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Editar(int id, [FromBody] InformacionAcademica info)
        {
            var ok = await _service.Editar(id, info);
            if (!ok) return NotFound();
            return NoContent();
        }

        // DELETE: api/InformacionAcademica/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(int id)
        {
            var ok = await _service.Eliminar(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}