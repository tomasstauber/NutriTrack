namespace NutriTrack.MAUI.Models
{
    // Respuesta de POST api/EventoSanitario/multiple (en el back: EventoSanitarioResponseDTO)
    public class EventoSanitarioRegistrado
    {
        public int CantidadAnimalesAlcanzados { get; set; }

        // Nombre del enum TipoEvento del back, sin tilde ("Desparasitacion")
        public string TipoEvento { get; set; } = string.Empty;

        public DateTime FechaEvento { get; set; }
    }
}
