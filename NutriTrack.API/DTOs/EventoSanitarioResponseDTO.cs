using System;

namespace NutriTrack.API.DTOs
{
    public class EventoSanitarioResponseDTO
    {
        public int CantidadAnimalesAlcanzados { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
    }
}