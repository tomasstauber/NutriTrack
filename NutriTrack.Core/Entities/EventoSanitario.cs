// Core.Entities/EventoSanitario.cs
using System;
using System.Collections.Generic;

namespace NutriTrack.Core.Entities
{
    public class EventoSanitario
    {
        public int Id { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }

        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }

        public List<DetalleMedicamento> DetallesMedicamento { get; set; } = new();
    }
}
