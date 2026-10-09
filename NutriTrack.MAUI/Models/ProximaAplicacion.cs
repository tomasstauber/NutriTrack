namespace NutriTrack.MAUI.Models
{
    // Fila de "Próximas aplicaciones" del reporte de fechas importantes (en el back: ProximaAplicacionItemDTO)
    public class ProximaAplicacion
    {
        // Ya viene armado como "CUIG-NRO"
        public string Destino { get; set; } = string.Empty;

        // Nombre del enum TipoEvento del back, sin tilde ("Vacunacion")
        public string TipoEvento { get; set; } = string.Empty;

        // Medicamentos del evento unidos con ", ". Texto vacío si el evento no tiene
        public string? Producto { get; set; }

        public DateTime FechaProximaAplicacion { get; set; }

        // Nombre completo del usuario que registró el evento
        public string Responsable { get; set; } = string.Empty;

        // No viene de la API: tipo con tilde
        public string TipoEventoTexto => TiposEvento.Texto(TipoEvento);
    }
}
