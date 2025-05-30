using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTOs;

namespace ProyectoNominas.API.Services
{
    public class ExpedienteService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ExpedienteService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<DocumentoExpedienteDto>> ObtenerDocumentosPorEmpleado(int empleadoId)
        {
            return await _context.DocumentosEmpleado
                .Where(x => x.EmpleadoId == empleadoId)
                .Select(x => new DocumentoExpedienteDto
                {
                    Id = x.Id,
                    TipoDocumento = x.TipoDocumento,
                    RutaArchivo = x.RutaArchivo,
                    FechaSubida = x.FechaSubida
                })
                .ToListAsync();
        }

        public async Task<DocumentoExpedienteDto?> SubirDocumento(CrearDocumentoExpedienteDto dto)
        {
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploads);

            var fileName = $"{Guid.NewGuid()}_{dto.Archivo.FileName}";
            var filePath = Path.Combine(uploads, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Archivo.CopyToAsync(stream);
            }

            var doc = new DocumentoEmpleado
            {
                EmpleadoId = dto.EmpleadoId,
                TipoDocumento = dto.TipoDocumento,
                RutaArchivo = $"/uploads/{fileName}",
                FechaSubida = DateTime.Now
            };

            _context.DocumentosEmpleado.Add(doc);
            await _context.SaveChangesAsync();

            return new DocumentoExpedienteDto
            {
                Id = doc.Id,
                TipoDocumento = doc.TipoDocumento,
                RutaArchivo = doc.RutaArchivo,
                FechaSubida = doc.FechaSubida
            };
        }

        public async Task<ExpedienteValidacionDto> ValidarExpediente(int empleadoId)
        {
            var documentosObligatorios = await _context.ConfiguracionesExpediente
                .Where(c => c.Obligatorio)
                .Select(c => c.TipoDocumento.ToLower())
                .ToListAsync();

            var documentosEmpleado = await _context.DocumentosEmpleado
                .Where(d => d.EmpleadoId == empleadoId)
                .Select(d => d.TipoDocumento.ToLower())
                .ToListAsync();

            var faltantes = documentosObligatorios
                .Where(doc => !documentosEmpleado.Contains(doc))
                .ToList();

            return new ExpedienteValidacionDto
            {
                ExpedienteCompleto = faltantes.Count == 0,
                DocumentosFaltantes = faltantes
            };
        }
    }
}