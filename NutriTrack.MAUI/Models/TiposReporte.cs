namespace NutriTrack.MAUI.Models
{
    // Tipos de período de los reportes (CU17 D2). Viajan así, en minúscula y sin tilde:
    // el back valida los tres reportes con la misma lista (PeriodoReporteCalculator.TiposValidos)
    public static class TiposReporte
    {
        public const string Actual = "actual";
        public const string Diario = "diario";
        public const string Semanal = "semanal";
        public const string Quincenal = "quincenal";
        public const string Mensual = "mensual";
        public const string Anual = "anual";
        public const string Personalizado = "personalizado";
    }
}
