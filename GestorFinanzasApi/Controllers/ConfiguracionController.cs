using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfiguracionController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public ConfiguracionController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Configuracion
        // GET: api/Configuracion?idDispositivo=1
        [HttpGet]
        public async Task<IActionResult> ObtenerConfiguraciones(
            [FromQuery] int? idDispositivo)
        {
            var consulta = _context.Configuraciones
                .AsNoTracking()
                .AsQueryable();

            if (idDispositivo.HasValue)
            {
                consulta = consulta.Where(c =>
                    c.IdDispositivo == idDispositivo.Value);
            }

            var configuraciones = await consulta.ToListAsync();

            return Ok(configuraciones);
        }

        // GET: api/Configuracion/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObtenerConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

            if (configuracion == null)
                return NotFound(new
                {
                    mensaje = "Configuración no encontrada."
                });

            return Ok(configuracion);
        }

        // POST: api/Configuracion
        [HttpPost]
        public async Task<IActionResult> CrearConfiguracion(
            [FromBody] Configuracion configuracion)
        {
            var dispositivo = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo ==
                    configuracion.IdDispositivo);

            if (!dispositivo)
                return BadRequest(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });

            var yaExiste = await _context.Configuraciones
                .AnyAsync(c => c.IdDispositivo ==
                    configuracion.IdDispositivo);

            if (yaExiste)
                return Conflict(new
                {
                    mensaje = "Este dispositivo ya tiene una configuración."
                });

            configuracion.IdConfiguracion = 0;

            if (string.IsNullOrWhiteSpace(configuracion.Moneda))
                configuracion.Moneda = "COP";

            if (string.IsNullOrWhiteSpace(configuracion.Tema))
                configuracion.Tema = "claro";

            if (string.IsNullOrWhiteSpace(configuracion.Idioma))
                configuracion.Idioma = "es";

            _context.Configuraciones.Add(configuracion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerConfiguracion),
                new { id = configuracion.IdConfiguracion },
                configuracion);
        }

        // PUT: api/Configuracion/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarConfiguracion(
            int id,
            [FromBody] Configuracion datos)
        {
            if (id != datos.IdConfiguracion)
                return BadRequest(new
                {
                    mensaje = "El ID de la URL no coincide con el ID de la configuración."
                });

            var configuracion = await _context.Configuraciones
                .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

            if (configuracion == null)
                return NotFound(new
                {
                    mensaje = "Configuración no encontrada."
                });

            var dispositivoExiste = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo == datos.IdDispositivo);

            if (!dispositivoExiste)
                return BadRequest(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });

            var otraConfiguracion = await _context.Configuraciones
                .AnyAsync(c =>
                    c.IdDispositivo == datos.IdDispositivo &&
                    c.IdConfiguracion != id);

            if (otraConfiguracion)
                return Conflict(new
                {
                    mensaje = "Este dispositivo ya tiene otra configuración."
                });

            configuracion.IdDispositivo = datos.IdDispositivo;
            configuracion.Moneda = string.IsNullOrWhiteSpace(datos.Moneda)
                ? "COP" : datos.Moneda;
            configuracion.Tema = string.IsNullOrWhiteSpace(datos.Tema)
                ? "claro" : datos.Tema;
            configuracion.Idioma = string.IsNullOrWhiteSpace(datos.Idioma)
                ? "es" : datos.Idioma;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Configuracion/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarConfiguracion(int id)
        {
            var configuracion = await _context.Configuraciones
                .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

            if (configuracion == null)
                return NotFound(new
                {
                    mensaje = "Configuración no encontrada."
                });

            _context.Configuraciones.Remove(configuracion);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
