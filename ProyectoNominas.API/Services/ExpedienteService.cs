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
            if (dto.Archivo == null || dto.Archivo.Length == 0)
                return null;

            // Carpeta donde guardar archivos
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads", "expediente");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Nombre único
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(dto.Archivo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            // Guardar archivo físicamente
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Archivo.CopyToAsync(stream);
            }

            // Ruta relativa para que el frontend la pueda consumir
            var rutaRelativa = $"/uploads/expediente/{fileName}";

            // Guardar en base de datos
            var doc = new DocumentoEmpleado
            {
                EmpleadoId = dto.EmpleadoId,
                TipoDocumento = dto.TipoDocumento,
                RutaArchivo = rutaRelativa,
                FechaSubida = DateTime.Now
            };
            _context.DocumentosEmpleado.Add(doc);
            await _context.SaveChangesAsync();

            // Retornar DTO para mostrar en frontend
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