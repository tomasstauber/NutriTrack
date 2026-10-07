namespace NutriTrack.MAUI.Models
{
    // Evento del historial sanitario de un animal
    // (GET api/EventoSanitario/animal/{idAnimal}, en el back: EventoHistorialDTO)
    public class EventoHistorial
    {
        public int Id { get; set; }

        // Texto (ej. "Vacunacion", "Desparasitacion")
        public string TipoEvento { get; set; } = string.Empty;

        public DateTime FechaEvento { get; set; }

        // Opcionales: null si el evento no los tiene
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }

        // Nombre completo del usuario que registró el evento
        public string? Responsable { get; set; }

        public List<MedicamentoHistorial> Medicamentos { get; set; } = [];
    }
}
