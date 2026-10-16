using GestorFinanzasAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResumenController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public ResumenController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Resumen/1
        // GET: api/Resumen/1?fechaInicio=2026-10-01&fechaFin=2026-10-31
        [HttpGet("{idDispositivo:int}")]
        public async Task<IActionResult> ObtenerResumen(
            int idDispositivo,
            [FromQuery] DateOnly? fechaInicio,
            [FromQuery] DateOnly? fechaFin)
        {
            var dispositivoExiste = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo == idDispositivo);

            if (!dispositivoExiste)
            {
                return NotFound(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });
            }

            if (fechaInicio.HasValue &&
                fechaFin.HasValue &&
                fechaInicio.Value > fechaFin.Value)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha inicial no puede ser posterior a la fecha final."
                });
            }

            var consulta = _context.Movimientos
                .AsNoTracking()
                .Where(m => m.IdDispositivo == idDispositivo);

            if (fechaInicio.HasValue)
            {
                consulta = consulta.Where(m =>
                    m.Fecha >= fechaInicio.Value);
            }

            if (fechaFin.HasValue)
            {
                consulta = consulta.Where(m =>
                    m.Fecha <= fechaFin.Value);
            }

            var ingresos = await consulta
                .Where(m => m.Tipo.ToLower() == "ingreso")
                .SumAsync(m => (decimal?)m.Monto) ?? 0m;

            var gastos = await consulta
                .Where(m => m.Tipo.ToLower() == "gasto")
                .SumAsync(m => (decimal?)m.Monto) ?? 0m;

            var cantidadMovimientos = await consulta.CountAsync();

            var resumen = new
            {
                idDispositivo = idDispositivo,
                fechaInicio = fechaInicio,
                fechaFin = fechaFin,
                totalIngresos = ingresos,
                totalGastos = gastos,
                saldo = ingresos - gastos,
                cantidadMovimientos = cantidadMovimientos
            };

            return Ok(resumen);
        }
    }
}