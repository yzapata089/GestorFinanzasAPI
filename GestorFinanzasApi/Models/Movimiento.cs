namespace GestorFinanzasAPI.Models
{
    public class Movimiento
    {
        public int IdMovimiento { get; set; }

        public int IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        public string Tipo { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public DateOnly Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;

        public DateTime Creado { get; set; }
        public DateTime Actualizado { get; set; }

        public int IdDispositivo { get; set; }
        public Dispositivo? Dispositivo { get; set; }
    }
}
