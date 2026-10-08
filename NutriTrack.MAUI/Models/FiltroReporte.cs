namespace NutriTrack.MAUI.Models
{
    // Filtro de período común a los tres reportes (en el back: ReporteInventarioAnimalesDTO,
    // ReporteFechasImportantesDTO y ReporteEvolucionPesoDTO). Viaja por query.
    // Lo arma FiltroPeriodoReporteViewModel con solo los datos que corresponden al tipo:
    // lo que queda en null no se manda
    public class FiltroReporte
    {
        // null: todos los rodeos
        public int? IdRodeo { get; init; }

        // Una constante de TiposReporte
        public string TipoReporte { get; init; } = string.Empty;

        // De diario a anual
        public DateTime? FechaReferencia { get; init; }

        // Personalizado
        public DateTime? Desde { get; init; }
        public DateTime? Hasta { get; init; }
    }
}
