using ProyectoNominas.API.Data;
using ProyectoNominas.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProyectoNominas.API.Services
{
    public class NominaService
    {
        private readonly ApplicationDbContext _context;

        // Puedes inyectar aquí también parámetros legales si los tienes en otra tabla/configuración.
        public NominaService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Crea una nómina para un periodo, calculando todos los conceptos
        public async Task<Nomina> GenerarNominaAsync(DateTime fechaInicio, DateTime fechaFin, string periodo)
        {
            var empleados = await _context.Empleados
                .Include(e => e.Puesto)
                .Where(e => e.EstadoLaboral == "Activo")
                .ToListAsync();

            // Aquí puedes cargar parámetros legales (IGSS, IRTRA, etc.) desde la BD si tienes
            decimal porcentajeIGSS = 0.0483M;     // Ejemplo: 4.83% para empleado
            decimal porcentajeIRTRA = 0.01M;      // Ejemplo: 1%
            decimal porcentajeISR = 0.05M;        // Ejemplo: 5%

            var nomina = new Nomina
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Periodo = periodo,
                Detalles = new List<DetalleNomina>()
            };

            decimal montoTotal = 0;

            foreach (var e in empleados)
            {
                // Obtén horas trabajadas y extras (aquí puedes ajustar según tu lógica)
                var horasTrabajadas = await _context.HorasTrabajadas
                    .Where(h => h.EmpleadoId == e.Id && h.Fecha >= fechaInicio && h.Fecha <= fechaFin)
                    .ToListAsync();
                var totalHorasExtras = horasTrabajadas.Sum(h => h.HorasExtras);

                // Calcula bonificaciones y comisiones si tienes lógica adicional

                // Descuentos legales
                decimal igss = e.Salario * porcentajeIGSS;
                decimal irtra = e.Salario * porcentajeIRTRA;
                decimal isr = e.Salario * porcentajeISR;

                // Suma otros descuentos si tienes lógica adicional
                decimal otrosDescuentos = 0;

                // Simulación de bonificaciones y comisiones
                decimal bonificaciones = 0;
                decimal comisiones = 0;

                // Total a pagar
                decimal totalPagar = e.Salario + bonificaciones + comisiones + (totalHorasExtras * 20) // puedes usar tu valor por hora extra
                                    - igss - irtra - isr - otrosDescuentos;

                var detalle = new DetalleNomina
                {
                    EmpleadoId = e.Id,
                    SalarioBase = e.Salario,
                    HorasExtras = totalHorasExtras,
                    Bonificaciones = bonificaciones,
                    Comisiones = comisiones,
                    Descuentos = otrosDescuentos,
                    IGSS = igss,
                    IRTRA = irtra,
                    ISR = isr,
                    TotalPagar = totalPagar
                };

                nomina.Detalles.Add(detalle);
                montoTotal += totalPagar;
            }

            nomina.MontoTotal = montoTotal;

            _context.Nominas.Add(nomina);
            await _context.SaveChangesAsync();

            return nomina;
        }

        // Obtener nómina por id, incluyendo detalles
        public async Task<Nomina?> ObtenerNominaPorIdAsync(int id)
        {
            return await _context.Nominas
                .Include(n => n.Detalles)
                    .ThenInclude(d => d.Empleado)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        // Listar nóminas
        public async Task<List<Nomina>> ListarNominasAsync()
        {
            return await _context.Nominas
                .Include(n => n.Detalles)
                .OrderByDescending(n => n.FechaInicio)
                .ToListAsync();
        }

        internal async Task ObtenerNominaDetalleAsync(int id)
        {
            throw new NotImplementedException();
        }

        internal async Task SimularNominaAsync(DateTime fechaInicio, DateTime fechaFin, string periodo)
        {
            throw new NotImplementedException();
        }
    }
}