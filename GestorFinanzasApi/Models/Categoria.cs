namespace GestorFinanzasAPI.Models
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Icono { get; set; }
        public bool Personalizada { get; set; }

        public int IdDispositivo { get; set; }
        public Dispositivo? Dispositivo { get; set; }

        public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
        public ICollection<Presupuesto> Presupuestos { get; set; } = new List<Presupuesto>();
    }
}
