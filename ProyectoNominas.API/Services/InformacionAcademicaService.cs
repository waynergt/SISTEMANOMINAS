using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProyectoNominas.API.Services
{
    public class InformacionAcademicaService
    {
        private readonly ApplicationDbContext _context;

        public InformacionAcademicaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<InformacionAcademica>> ObtenerPorEmpleado(int empleadoId)
        {
            return await _context.InformacionesAcademicas
                .Where(i => i.EmpleadoId == empleadoId)
                .ToListAsync();
        }

        public async Task<InformacionAcademica?> ObtenerPorId(int id)
        {
            return await _context.InformacionesAcademicas.FindAsync(id);
        }

        public async Task<bool> Crear(InformacionAcademica info)
        {
            // Ejemplo de validación: no repetir título para mismo empleado
            bool yaExiste = await _context.InformacionesAcademicas
                .AnyAsync(x => x.EmpleadoId == info.EmpleadoId && x.Titulo == info.Titulo);

            if (yaExiste) return false;

            _context.InformacionesAcademicas.Add(info);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Editar(int id, InformacionAcademica info)
        {
            var existente = await _context.InformacionesAcademicas.FindAsync(id);
            if (existente == null) return false;

            // Ejemplo de validación: puedes agregar más validaciones aquí
            existente.Titulo = info.Titulo;
            existente.Institucion = info.Institucion;
            existente.FechaGraduacion = info.FechaGraduacion;
            existente.EmpleadoId = info.EmpleadoId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Eliminar(int id)
        {
            var existente = await _context.InformacionesAcademicas.FindAsync(id);
            if (existente == null) return false;

            _context.InformacionesAcademicas.Remove(existente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}