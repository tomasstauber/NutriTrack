using System;

namespace NutriTrack.API.DTOs
{
    public class ReporteFechasImportantesDTO
    {
        public int? IdRodeo { get; set; }
        public string? TipoReporte { get; set; }
        public DateTime? FechaReferencia { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}