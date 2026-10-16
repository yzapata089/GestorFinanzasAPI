using Microsoft.Extensions.Configuration;

namespace GestorFinanzasAPI.Models
{
    public class Dispositivo
    {
        public int IdDispositivo { get; set; }
        public string Identificador { get; set; } = string.Empty;

        public ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
        public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
        public Configuracion? Configuracion { get; set; }
    }
}
