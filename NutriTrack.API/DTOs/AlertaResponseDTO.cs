using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class AlertaResponseDTO
    {
        public int DiasAviso { get; set; }
        public int Total { get; set; }
        public List<AlertaItemDTO> Alertas { get; set; } = new();
    }

    public class AlertaItemDTO
    {
        // "Sanitaria" / "PlanAlimenticio"
        public string Tipo { get; set; }
        // "ProximaAplicacion" / "Vencimiento"
        public string Subtipo { get; set; }
        // Caravana del animal o nombre del rodeo
        public string Destino { get; set; }
        public string Descripcion { get; set; }
        public DateOnly Fecha { get; set; }
        public int DiasRestantes { get; set; }
        // Solo planes; null en las sanitarias
        public DateOnly? VigenciaDesde { get; set; }
        public int? CantidadAnimales { get; set; }
    }
}
