using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientoController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public MovimientoController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        // Ejemplo: api/Movimientos?idDispositivo=1&tipo=gasto
        [HttpGet]
        public async Task<IActionResult> GetMovimientos(
            [FromQuery] int? idDispositivo,
            [FromQuery] string? tipo,
            [FromQuery] DateOnly? fechaInicio,
            [FromQuery] DateOnly? fechaFin)
        {
            if (fechaInicio.HasValue && fechaFin.HasValue &&
                fechaInicio.Value > fechaFin.Value)
            {
                return BadRequest(new
                {
                    mensaje = "La fecha inicial no puede ser posterior a la fecha final."
                });
            }

            var consulta = _context.Movimientos.AsNoTracking();

            if (idDispositivo.HasValue)
                consulta = consulta.Where(
                    m => m.IdDispositivo == idDispositivo.Value);

            if (!string.IsNullOrWhiteSpace(tipo))
            {
                tipo = tipo.Trim().ToLowerInvariant();

                if (tipo != "ingreso" && tipo != "gasto")
                    return BadRequest(new
                    {
                        mensaje = "El tipo debe ser ingreso o gasto."
                    });

                consulta = consulta.Where(m => m.Tipo == tipo);
            }

            if (fechaInicio.HasValue)
                consulta = consulta.Where(
                    m => m.Fecha >= fechaInicio.Value);

            if (fechaFin.HasValue)
                consulta = consulta.Where(
                    m => m.Fecha <= fechaFin.Value);

            var movimientos = await consulta
                .OrderByDescending(m => m.Fecha)
                .Select(m => new
                {
                    m.IdMovimiento,
                    m.IdCategoria,
                    m.Tipo,
                    m.Monto,
                    m.Fecha,
                    m.Descripcion,
                    m.Creado,
                    m.Actualizado,
                    m.IdDispositivo
                })
                .ToListAsync();

            return Ok(movimientos);
        }

        // GET: api/Movimientos/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMovimiento(int id)
        {
            var movimiento = await _context.Movimientos
                .AsNoTracking()
                .Where(m => m.IdMovimiento == id)
                .Select(m => new
                {
                    m.IdMovimiento,
                    m.IdCategoria,
                    m.Tipo,
                    m.Monto,
                    m.Fecha,
                    m.Descripcion,
                    m.Creado,
                    m.Actualizado,
                    m.IdDispositivo
                })
                .FirstOrDefaultAsync();

            if (movimiento == null)
                return NotFound(new
                {
                    mensaje = "Movimiento no encontrado."
                });

            return Ok(movimiento);
        }

        // POST: api/Movimientos
        [HttpPost]
        public async Task<IActionResult> CrearMovimiento(
            [FromBody] Movimiento movimiento)
        {
            string tipo = movimiento.Tipo?.Trim().ToLowerInvariant() ?? "";

            if (tipo != "ingreso" && tipo != "gasto")
                return BadRequest(new
                {
                    mensaje = "El tipo debe ser ingreso o gasto."
                });

            if (movimiento.Monto <= 0)
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });

            if (string.IsNullOrWhiteSpace(movimiento.Descripcion))
                movimiento.Descripcion = "";

            bool dispositivoExiste = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo == movimiento.IdDispositivo);

            if (!dispositivoExiste)
                return BadRequest(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.IdCategoria == movimiento.IdCategoria &&
                    c.IdDispositivo == movimiento.IdDispositivo);

            if (categoria == null)
                return BadRequest(new
                {
                    mensaje = "La categoría no existe para ese dispositivo."
                });

            if (categoria.Tipo != "ambos" && categoria.Tipo != tipo)
                return BadRequest(new
                {
                    mensaje = "El tipo de movimiento no coincide con la categoría."
                });

            movimiento.Tipo = tipo;
            movimiento.Descripcion = movimiento.Descripcion.Trim();
            movimiento.Creado = DateTime.Now;
            movimiento.Actualizado = DateTime.Now;

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMovimiento),
                new { id = movimiento.IdMovimiento },
                new
                {
                    movimiento.IdMovimiento,
                    movimiento.IdCategoria,
                    movimiento.Tipo,
                    movimiento.Monto,
                    movimiento.Fecha,
                    movimiento.Descripcion,
                    movimiento.Creado,
                    movimiento.Actualizado,
                    movimiento.IdDispositivo
                });
        }

        // PUT: api/Movimientos/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarMovimiento(
            int id,
            [FromBody] Movimiento datos)
        {
            if (id != datos.IdMovimiento)
                return BadRequest(new
                {
                    mensaje = "El ID de la URL debe coincidir con el ID del cuerpo."
                });

            string tipo = datos.Tipo?.Trim().ToLowerInvariant() ?? "";

            if (tipo != "ingreso" && tipo != "gasto")
                return BadRequest(new
                {
                    mensaje = "El tipo debe ser ingreso o gasto."
                });

            if (datos.Monto <= 0)
                return BadRequest(new
                {
                    mensaje = "El monto debe ser mayor que cero."
                });

            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
                return NotFound(new
                {
                    mensaje = "Movimiento no encontrado."
                });

            var categoria = await _context.Categorias
                .AsNoTracking()
                .FirstOrDefaultAsync(c =>
                    c.IdCategoria == datos.IdCategoria &&
                    c.IdDispositivo == movimiento.IdDispositivo);

            if (categoria == null)
                return BadRequest(new
                {
                    mensaje = "La categoría no existe para ese dispositivo."
                });

            if (categoria.Tipo != "ambos" && categoria.Tipo != tipo)
                return BadRequest(new
                {
                    mensaje = "El tipo de movimiento no coincide con la categoría."
                });

            movimiento.IdCategoria = datos.IdCategoria;
            movimiento.Tipo = tipo;
            movimiento.Monto = datos.Monto;
            movimiento.Fecha = datos.Fecha;
            movimiento.Descripcion = datos.Descripcion?.Trim() ?? "";
            movimiento.Actualizado = DateTime.Now;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Movimientos/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
                return NotFound(new
                {
                    mensaje = "Movimiento no encontrado."
                });

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
