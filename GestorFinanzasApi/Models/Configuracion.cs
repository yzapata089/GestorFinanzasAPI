namespace GestorFinanzasAPI.Models
{
    public class Configuracion
    {
        public int IdConfiguracion { get; set; }

        public string Moneda { get; set; } = "COP";
        public string Tema { get; set; } = "claro";
        public string Idioma { get; set; } = "es";

        public int IdDispositivo { get; set; }
        public Dispositivo? Dispositivo { get; set; }
    }
}
