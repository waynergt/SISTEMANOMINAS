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
        private readonly IWebHostEnvironment _env;

        public DocumentoEmpleadoController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/DocumentoEmpleado
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentoEmpleadoDto>>> GetDocumentos()
        {
            return await _context.DocumentosEmpleado
                .Include(d => d.Empleado)
                .Select(d => new DocumentoEmpleadoDto
                {
                    Id = d.Id,
                    EmpleadoId = d.EmpleadoId,
                    TipoDocumento = d.TipoDocumento,
                    UrlArchivo = d.UrlArchivo,
                    FechaSubida = d.FechaSubida
                })
                .ToListAsync();
        }

        // GET: api/DocumentoEmpleado/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DocumentoEmpleadoDto>> GetDocumento(int id)
        {
            var d = await _context.DocumentosEmpleado
                .Include(d => d.Empleado)
                .Where(d => d.Id == id)
                .Select(d => new DocumentoEmpleadoDto
                {
                    Id = d.Id,
                    EmpleadoId = d.EmpleadoId,
                    TipoDocumento = d.TipoDocumento,
                    UrlArchivo = d.UrlArchivo,
                    FechaSubida = d.FechaSubida
                }).FirstOrDefaultAsync();

            if (d == null)
                return NotFound();

            return d;
        }

        // GET: api/DocumentoEmpleado/empleado/5
        [HttpGet("empleado/{empleadoId}")]
        public async Task<ActionResult<IEnumerable<DocumentoEmpleadoDto>>> GetDocumentosEmpleado(int empleadoId)
        {
            return await _context.DocumentosEmpleado
                .Where(d => d.EmpleadoId == empleadoId)
                .Select(d => new DocumentoEmpleadoDto
                {
                    Id = d.Id,
                    EmpleadoId = d.EmpleadoId,
                    TipoDocumento = d.TipoDocumento,
                    UrlArchivo = d.UrlArchivo,
                    FechaSubida = d.FechaSubida
                })
                .ToListAsync();
        }

        // POST: api/DocumentoEmpleado
        [HttpPost]
        public async Task<ActionResult<DocumentoEmpleadoDto>> PostDocumento([FromForm] DocumentoEmpleadoCreateDto dto)
        {
            if (dto.Archivo == null || dto.Archivo.Length == 0)
                return BadRequest("Debe subir un archivo.");

            var uploads = Path.Combine(_env.WebRootPath, "expedientes");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);

            var nombreArchivo = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
            var rutaArchivo = Path.Combine(uploads, nombreArchivo);

            using (var stream = System.IO.File.Create(rutaArchivo))
            {
                await dto.Archivo.CopyToAsync(stream);
            }

            var documento = new DocumentoEmpleado
            {
                EmpleadoId = dto.EmpleadoId,
                TipoDocumento = dto.TipoDocumento,
                UrlArchivo = $"/expedientes/{nombreArchivo}",
                FechaSubida = DateTime.UtcNow
            };
            _context.DocumentosEmpleado.Add(documento);
            await _context.SaveChangesAsync();

            var result = new DocumentoEmpleadoDto
            {
                Id = documento.Id,
                EmpleadoId = documento.EmpleadoId,
                TipoDocumento = documento.TipoDocumento,
                UrlArchivo = documento.UrlArchivo,
                FechaSubida = documento.FechaSubida
            };

            return CreatedAtAction(nameof(GetDocumento), new { id = documento.Id }, result);
        }

        public class DocumentoEmpleadoDto
        {
            public int Id { get; set; }
            public int EmpleadoId { get; set; }
            public string? TipoDocumento { get; set; }
            public string? UrlArchivo { get; set; }
            public DateTime FechaSubida { get; set; }
        }

        public class DocumentoEmpleadoCreateDto
        {
            public int EmpleadoId { get; set; }
            public string? TipoDocumento { get; set; }
            public IFormFile? Archivo { get; set; }
        }
    }
}