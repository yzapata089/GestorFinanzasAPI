using System.Text.Json.Serialization;

namespace GestorFinanzasAPI.Models
{
    public class Abono
    {
        public int IdAbono { get; set; }

        public int IdPresupuesto { get; set; }
        [JsonIgnore]
        public Presupuesto? Presupuesto { get; set; }

        public decimal Monto { get; set; }
        public DateOnly Fecha { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    }
}
