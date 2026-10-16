namespace GestorFinanzasAPI.Models
{
    public class Presupuesto
    {
        public int IdPresupuesto { get; set; }

        public int IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        public decimal Monto { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public DateOnly FechaInicio { get; set; }
        public DateOnly FechaFin { get; set; }

        public int IdDispositivo { get; set; }
        public Dispositivo? Dispositivo { get; set; }

        public ICollection<Abono> Abonos { get; set; } = new List<Abono>();
    }
}
