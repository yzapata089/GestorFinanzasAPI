using GestorFinanzasAPI.Data;
using GestorFinanzasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorFinanzasAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly FinanzasDbContext _context;

        public CategoriaController(FinanzasDbContext context)
        {
            _context = context;
        }

        // GET: api/Categorias
        [HttpGet]
        public async Task<IActionResult> GetCategorias(
            [FromQuery] int? idDispositivo)
        {
            var consulta = _context.Categorias.AsNoTracking();

            if (idDispositivo.HasValue)
            {
                consulta = consulta.Where(
                    c => c.IdDispositivo == idDispositivo.Value);
            }

            var categorias = await consulta
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    c.Tipo,
                    c.Color,
                    c.Icono,
                    c.Personalizada,
                    c.IdDispositivo
                })
                .ToListAsync();

            return Ok(categorias);
        }

        // GET: api/Categorias/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCategoria(int id)
        {
            var categoria = await _context.Categorias
                .AsNoTracking()
                .Where(c => c.IdCategoria == id)
                .Select(c => new
                {
                    c.IdCategoria,
                    c.Nombre,
                    c.Tipo,
                    c.Color,
                    c.Icono,
                    c.Personalizada,
                    c.IdDispositivo
                })
                .FirstOrDefaultAsync();

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            return Ok(categoria);
        }

        // POST: api/Categorias
        [HttpPost]
        public async Task<IActionResult> CrearCategoria(
            [FromBody] Categoria categoria)
        {
            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre es obligatorio."
                });
            }

            string tipo = categoria.Tipo.Trim().ToLowerInvariant();

            if (tipo != "ingreso" &&
                tipo != "gasto" &&
                tipo != "ambos")
            {
                return BadRequest(new
                {
                    mensaje = "El tipo debe ser ingreso, gasto o ambos."
                });
            }

            bool dispositivoExiste = await _context.Dispositivos
                .AnyAsync(d => d.IdDispositivo == categoria.IdDispositivo);

            if (!dispositivoExiste)
            {
                return BadRequest(new
                {
                    mensaje = "El dispositivo indicado no existe."
                });
            }

            string nombre = categoria.Nombre.Trim();

            bool duplicada = await _context.Categorias.AnyAsync(
                c => c.IdDispositivo == categoria.IdDispositivo &&
                     c.Nombre == nombre);

            if (duplicada)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe una categoría con ese nombre para el dispositivo."
                });
            }

            categoria.Nombre = nombre;
            categoria.Tipo = tipo;

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategoria),
                new { id = categoria.IdCategoria },
                new
                {
                    categoria.IdCategoria,
                    categoria.Nombre,
                    categoria.Tipo,
                    categoria.Color,
                    categoria.Icono,
                    categoria.Personalizada,
                    categoria.IdDispositivo
                });
        }

        // PUT: api/Categorias/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ActualizarCategoria(
            int id,
            [FromBody] Categoria datos)
        {
            if (id != datos.IdCategoria)
            {
                return BadRequest(new
                {
                    mensaje = "El ID de la URL debe coincidir con el ID del cuerpo."
                });
            }

            if (string.IsNullOrWhiteSpace(datos.Nombre))
            {
                return BadRequest(new
                {
                    mensaje = "El nombre es obligatorio."
                });
            }

            string tipo = datos.Tipo.Trim().ToLowerInvariant();

            if (tipo != "ingreso" &&
                tipo != "gasto" &&
                tipo != "ambos")
            {
                return BadRequest(new
                {
                    mensaje = "El tipo debe ser ingreso, gasto o ambos."
                });
            }

            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            string nombre = datos.Nombre.Trim();

            bool duplicada = await _context.Categorias.AnyAsync(
                c => c.IdDispositivo == categoria.IdDispositivo &&
                     c.Nombre == nombre &&
                     c.IdCategoria != id);

            if (duplicada)
            {
                return Conflict(new
                {
                    mensaje = "Ya existe otra categoría con ese nombre."
                });
            }

            categoria.Nombre = nombre;
            categoria.Tipo = tipo;
            categoria.Color = datos.Color;
            categoria.Icono = datos.Icono;
            categoria.Personalizada = datos.Personalizada;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categorias/1
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);

            if (categoria == null)
            {
                return NotFound(new
                {
                    mensaje = "Categoría no encontrada."
                });
            }

            bool tieneMovimientos = await _context.Movimientos
                .AnyAsync(m => m.IdCategoria == id);

            bool tienePresupuestos = await _context.Presupuestos
                .AnyAsync(p => p.IdCategoria == id);

            if (tieneMovimientos || tienePresupuestos)
            {
                return Conflict(new
                {
                    mensaje = "No se puede eliminar la categoría porque tiene movimientos o presupuestos relacionados."
                });
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

