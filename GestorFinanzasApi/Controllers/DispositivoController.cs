using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class DispositivoController : ControllerBase
        {
            private readonly FinanzasDbContext _context;

            public DispositivoController(FinanzasDbContext context)
            {
                _context = context;
            }

            // GET: api/Dispositivos
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Dispositivo>>> GetDispositivos()
            {
                var dispositivos = await _context.Dispositivos
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(dispositivos);
            }

            // GET: api/Dispositivos/5
            [HttpGet("{id:int}")]
            public async Task<ActionResult<Dispositivo>> GetDispositivo(int id)
            {
                var dispositivo = await _context.Dispositivos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.IdDispositivo == id);

                if (dispositivo == null)
                    return NotFound(new { mensaje = "Dispositivo no encontrado." });

                return Ok(dispositivo);
            }

            // POST: api/Dispositivos
            [HttpPost]
            public async Task<ActionResult<Dispositivo>> CrearDispositivo(
                [FromBody] Dispositivo dispositivo)
            {
                if (string.IsNullOrWhiteSpace(dispositivo.Identificador))
                    return BadRequest(new { mensaje = "El identificador es obligatorio." });

                dispositivo.Identificador = dispositivo.Identificador.Trim();

                bool existe = await _context.Dispositivos
                    .AnyAsync(d => d.Identificador == dispositivo.Identificador);

                if (existe)
                    return Conflict(new { mensaje = "Ese identificador ya existe." });

                _context.Dispositivos.Add(dispositivo);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetDispositivo),
                    new { id = dispositivo.IdDispositivo },
                    dispositivo);
            }

            // PUT: api/Dispositivos/5
            [HttpPut("{id:int}")]
            public async Task<IActionResult> ActualizarDispositivo(
                int id, [FromBody] Dispositivo datos)
            {
                if (id != datos.IdDispositivo)
                    return BadRequest(new { mensaje = "El ID de la URL no coincide con el del cuerpo." });

                if (string.IsNullOrWhiteSpace(datos.Identificador))
                    return BadRequest(new { mensaje = "El identificador es obligatorio." });

                var dispositivo = await _context.Dispositivos
                    .FindAsync(id);

                if (dispositivo == null)
                    return NotFound(new { mensaje = "Dispositivo no encontrado." });

                string identificador = datos.Identificador.Trim();

                bool duplicado = await _context.Dispositivos.AnyAsync(
                    d => d.Identificador == identificador &&
                         d.IdDispositivo != id);

                if (duplicado)
                    return Conflict(new { mensaje = "Ese identificador ya existe." });

                dispositivo.Identificador = identificador;

                await _context.SaveChangesAsync();

                return NoContent();
            }

            // DELETE: api/Dispositivos/5
            [HttpDelete("{id:int}")]
            public async Task<IActionResult> EliminarDispositivo(int id)
            {
                var dispositivo = await _context.Dispositivos.FindAsync(id);

                if (dispositivo == null)
                    return NotFound(new { mensaje = "Dispositivo no encontrado." });

                _context.Dispositivos.Remove(dispositivo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }
    
}
