using Microsoft.AspNetCore.Mvc;
using ProyectoNominas.API.Services;
using ProyectoNominas.API.Domain.Entities;

namespace ProyectoNominas.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NominaController : ControllerBase
    {
        private readonly NominaService _service;

        public NominaController(NominaService service)
        {
            _service = service;
        }

        // POST: api/Nomina/generar
        [HttpPost("generar")]
        public async Task<ActionResult<Nomina>> GenerarNomina([FromBody] GenerarNominaRequest request)
        {
            var nomina = await _service.GenerarNominaAsync(request.FechaInicio, request.FechaFin, request.Periodo);
            return Ok(nomina);
        }

        // GET: api/Nomina
        [HttpGet]
        public async Task<ActionResult<List<Nomina>>> GetNominas()
        {
            var nominas = await _service.ListarNominasAsync();
            return Ok(nominas);
        }

        // GET: api/Nomina/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Nomina>> GetNomina(int id)
        {
            var nomina = await _service.ObtenerNominaPorIdAsync(id);
            if (nomina == null) return NotFound();
            return Ok(nomina);
        }
    }

    // DTO para recibir el request de generación de nómina
    public class GenerarNominaRequest
    {
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Periodo { get; set; } = "";
    }
}