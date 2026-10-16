using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresupuestoController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public PresupuestoController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Presupuestos
        [HttpGet]
        public async Task<IActionResult> GetPresupuestos(
            [FromQuery] int? idDispositivo)
        {
            var consulta = _context.Presupuestos.AsNoTracking();

            if (idDispositivo.HasValue)
            {
                consulta = consulta.Where(
                    p => p.IdDispositivo == idDispositivo.Value);
            }

            var presupuestos = await consulta
                .OrderByDescending(p => p.FechaInicio)
                .Select(p => new
                {
                    p.IdPresupuesto,
                    p.IdCategoria,
                    p.Monto,
                    p.Periodo,
                    p.FechaInicio,
                    p.FechaFin,
                    p.IdDispositivo
                })
                .ToListAsync();

            return Ok(presupuestos);
        }

        // GET: api/Presupuestos/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPresupuesto(int id)
        {
            var presupuesto = await _context.Presupuestos
                .AsNoTracking()
                .Where(p => p.IdPresupuesto == id)
                .Select(p => new
                {
                    p.IdPresupuesto,
                    p.IdCategoria,
                    p.Monto,
                    p.Periodo,
                    p.FechaInicio,
                    p.FechaFin,
                    p.IdDispositivo
                })
                .FirstOrDefaultAsync();

            if (presupuesto == null)
            {
                return NotFound(new
                {
                    mensaje = "Presupuesto no encontrado."
                });
            }

            return Ok(presupuesto);
        }

        // POST: api/Presupuestos
        [HttpPost]
        public async Task<IActionResult> CrearPresupuesto(
            [FromBody] Presupuesto presupuesto)
        {
            if (presupuesto.Monto <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });
            }

            if (presupuesto.FechaFin < presupuesto.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha final no puede ser anterior a la inicial."
                });
            }

            if (string.IsNullOrWhiteSpace(presupuesto.Periodo))
            {
                return BadRequest(new
                {
                    mensaje = "El período es obligatorio."
                });
            }

            bool dispositivoExiste = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo == presupuesto.IdDispositivo);

            if (!dispositivoExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });
            }

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.IdCategoria == presupuesto.IdCategoria &&
                    c.IdDispositivo == presupuesto.IdDispositivo);

            if (categoria == null)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría no existe para ese dispositivo."
                });
            }

            if (categoria.Tipo == "ingreso")
            {
                return BadRequest(new
                {
                    mensaje = "No se puede crear un presupuesto de gasto para una categoría de ingreso."
                });
            }

            _context.Presupuestos.Add(presupuesto);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPresupuesto),
                new { id = presupuesto.IdPresupuesto },
                new
                {
                    presupuesto.IdPresupuesto,
                    presupuesto.IdCategoria,
                    presupuesto.Monto,
                    presupuesto.Periodo,
                    presupuesto.FechaInicio,
                    presupuesto.FechaFin,
                    presupuesto.IdDispositivo
                });
        }

        // PUT: api/Presupuestos/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarPresupuesto(
            int id,
            [FromBody] Presupuesto datos)
        {
            if (id != datos.IdPresupuesto)
            {
                return BadRequest(new
                {
                    mensaje = "El ID de la URL debe coincidir con el ID del cuerpo."
                });
            }

            if (datos.Monto <= 0)
            {
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });
            }

            if (datos.FechaFin < datos.FechaInicio)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha final no puede ser anterior a la inicial."
                });
            }

            if (string.IsNullOrWhiteSpace(datos.Periodo))
            {
                return BadRequest(new
                {
                    mensaje = "El período es obligatorio."
                });
            }

            var presupuesto = await _context.Presupuestos.FindAsync(id);

            if (presupuesto == null)
            {
                return NotFound(new
                {
                    mensaje = "Presupuesto no encontrado."
                });
            }

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.IdCategoria == datos.IdCategoria &&
                    c.IdDispositivo == presupuesto.IdDispositivo);

            if (categoria == null)
            {
                return BadRequest(new
                {
                    mensaje = "La categoría no existe para ese dispositivo."
                });
            }

            if (categoria.Tipo == "ingreso")
            {
                return BadRequest(new
                {
                    mensaje = "No se puede usar una categoría de ingreso para este presupuesto."
                });
            }

            presupuesto.IdCategoria = datos.IdCategoria;
            presupuesto.Monto = datos.Monto;
            presupuesto.Periodo = datos.Periodo.Trim();
            presupuesto.FechaInicio = datos.FechaInicio;
            presupuesto.FechaFin = datos.FechaFin;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Presupuestos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarPresupuesto(int id)
        {
            var presupuesto = await _context.Presupuestos.FindAsync(id);

            if (presupuesto == null)
            {
                return NotFound(new
                {
                    mensaje = "Presupuesto no encontrado."
                });
            }

            bool tieneAbonos = await _context.Abonos
                .AnyAsync(a => a.IdPresupuesto == id);

            if (tieneAbonos)
            {
                return Conflict(new
                {
                    mensaje = "No se puede eliminar el presupuesto porque tiene abonos relacionados."
                });
            }

            _context.Presupuestos.Remove(presupuesto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
