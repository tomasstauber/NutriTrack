namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/EventoSanitario/multiple (en el back: RegistrarEventoSanitarioMultipleDTO).
    // El responsable lo toma el back del token: no se envía.
    public class RegistrarEventoSanitarioRequest
    {
        // Rodeo completo y Selección manual: obligatorio. Selección libre: null
        public int? IdRodeo { get; set; }

        // Una constante de ModosSeleccionEvento
        public string ModoSeleccion { get; set; } = string.Empty;

        // Selección manual y libre: los animales elegidos. Rodeo completo: null
        public List<CaravanaRequest>? Caravanas { get; set; }

        // Nombre del enum TipoEvento del back, sin tilde ("Vacunacion")
        public string TipoEvento { get; set; } = string.Empty;

        // DateOnly viaja en ISO solo fecha ("2026-10-07")
        public DateOnly FechaEvento { get; set; }

        // Cero, uno o varios medicamentos. Sin medicamentos viaja vacía
        public List<DetalleMedicamentoRequest> DetallesMedicamento { get; set; } = [];

        // Opcionales: sin cargar viajan en null
        public DateOnly? VigenciaHasta { get; set; }
        public DateOnly? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
    }
}
