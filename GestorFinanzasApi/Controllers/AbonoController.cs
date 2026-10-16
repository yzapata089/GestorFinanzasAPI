using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonoController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public AbonoController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Abonos
        // GET: api/Abonos?idPresupuesto=1
        [HttpGet]
        public async Task<IActionResult> ObtenerAbonos(
            [FromQuery] int? idPresupuesto)
        {
            var consulta = _context.Abonos
                .AsNoTracking()
                .AsQueryable();

            if (idPresupuesto.HasValue)
            {
                consulta = consulta.Where(a =>
                    a.IdPresupuesto == idPresupuesto.Value);
            }

            var abonos = await consulta
                .OrderByDescending(a => a.Fecha)
                .ToListAsync();

            return Ok(abonos);
        }

        // GET: api/Abonos/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerAbono(int id)
        {
            var abono = await _context.Abonos
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.IdAbono == id);

            if (abono == null)
                return NotFound(new { mensaje = "Abono no encontrado." });

            return Ok(abono);
        }

        // POST: api/Abonos
        [HttpPost]
        public async Task<IActionResult> CrearAbono([FromBody] Abono abono)
        {
            if (abono.Monto <= 0)
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });

            var presupuesto = await _context.Presupuestos
                .FirstOrDefaultAsync(p =>
                    p.IdPresupuesto == abono.IdPresupuesto);

            if (presupuesto == null)
                return BadRequest(new
                {
                    mensaje = "El presupuesto indicado no existe."
                });

            if (abono.Fecha < presupuesto.FechaInicio ||
                abono.Fecha > presupuesto.FechaFin)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha del abono debe estar dentro del período del presupuesto."
                });
            }

            abono.IdAbono = 0;

            _context.Abonos.Add(abono);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerAbono),
                new { id = abono.IdAbono },
                abono);
        }

        // PUT: api/Abonos/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarAbono(
            int id,
            [FromBody] Abono datos)
        {
            if (id != datos.IdAbono)
                return BadRequest(new
                {
                    mensaje = "El ID de la URL no coincide con el ID del abono."
                });

            if (datos.Monto <= 0)
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });

            var abono = await _context.Abonos
                .FirstOrDefaultAsync(a => a.IdAbono == id);

            if (abono == null)
                return NotFound(new { mensaje = "Abono no encontrado." });

            var presupuesto = await _context.Presupuestos
                .FirstOrDefaultAsync(p =>
                    p.IdPresupuesto == datos.IdPresupuesto);

            if (presupuesto == null)
                return BadRequest(new
                {
                    mensaje = "El presupuesto indicado no existe."
                });

            if (datos.Fecha < presupuesto.FechaInicio ||
                datos.Fecha > presupuesto.FechaFin)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha del abono debe estar dentro del período del presupuesto."
                });
            }

            abono.IdPresupuesto = datos.IdPresupuesto;
            abono.Monto = datos.Monto;
            abono.Fecha = datos.Fecha;
            abono.Descripcion = datos.Descripcion;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Abonos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarAbono(int id)
        {
            var abono = await _context.Abonos
                .FirstOrDefaultAsync(a => a.IdAbono == id);

            if (abono == null)
                return NotFound(new { mensaje = "Abono no encontrado." });

            _context.Abonos.Remove(abono);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
