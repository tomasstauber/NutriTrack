using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class RegistrarEventoSanitarioMultipleDTO
    {
        public int? IdRodeo { get; set; } //ahora es nullable pero es obligatorio solo en "Rodeo completo" y "Selección manual"
        public string ModoSeleccion { get; set; } = string.Empty; //"Rodeo completo", "Selección manual", "Selección libre"
        public List<CaravanaDTO>? Caravanas { get; set; } 
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
        public List<DetalleMedicamentoDTO>? DetallesMedicamento { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
    }
}