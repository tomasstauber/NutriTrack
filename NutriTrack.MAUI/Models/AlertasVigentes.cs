namespace NutriTrack.MAUI.Models
{
    // Respuesta de GET api/Alerta (en el back: AlertaResponseDTO).
    // Sin alertas responde 200 con la lista vacía, nunca 404
    public class AlertasVigentes
    {
        // Días hacia adelante que mira el back (Alertas:DiasAviso, 7 por defecto)
        public int DiasAviso { get; set; }

        public int Total { get; set; }

        // Ya vienen ordenadas por fecha: no se reordenan en el front
        public List<Alerta> Alertas { get; set; } = [];

        // No viene de la API: texto de lista vacía, el mismo en la lista y en la tarjeta del panel
        public string TextoSinAlertas => $"No hay alertas para los próximos {DiasAviso} días";
    }
}
