using Microsoft.EntityFrameworkCore;
using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using ProyectoNominas.API.DTOs;
using Microsoft.AspNetCore.Hosting;

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

            var carpetaDestino = Path.Combine(_env.WebRootPath ?? "wwwroot", "documentos");

            if (!Directory.Exists(carpetaDestino))
                Directory.CreateDirectory(carpetaDestino);

            var nombreLimpio = Path.GetFileName(dto.Archivo.FileName).Replace(" ", "_");
            var nombreArchivo = $"{Guid.NewGuid()}_{dto.TipoDocumento}_{nombreLimpio}";
            var rutaFisica = Path.Combine(carpetaDestino, nombreArchivo);

            using (var stream = new FileStream(rutaFisica, FileMode.Create))
            {
                await dto.Archivo.CopyToAsync(stream);
            }

            var rutaPublica = $"/documentos/{nombreArchivo}";

            var doc = new DocumentoEmpleado
            {
                EmpleadoId = dto.EmpleadoId,
                TipoDocumento = dto.TipoDocumento,
                RutaArchivo = rutaPublica,
                FechaSubida = DateTime.Now
            };

            _context.DocumentosEmpleado.Add(doc);
            await _context.SaveChangesAsync();

            return new DocumentoExpedienteDto
            {
                Id = doc.Id,
                TipoDocumento = doc.TipoDocumento,
                RutaArchivo = rutaPublica,
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
