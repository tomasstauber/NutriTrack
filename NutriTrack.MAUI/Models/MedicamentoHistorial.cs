namespace NutriTrack.MAUI.Models
{
    // Medicamento aplicado en un evento del historial (en el back: MedicamentoHistorialDTO)
    // Todos los datos pueden venir en null
    public class MedicamentoHistorial
    {
        public string? Nombre { get; set; }
        public decimal? Dosis { get; set; }
        public string? Unidad { get; set; }
        public string? Observaciones { get; set; }

        // Dosis y unidad para mostrar ("5 ml"); vacío si no hay dosis. No viene de la API: se arma acá
        public string TextoDosis => Dosis is { } dosis
            ? $"{dosis:0.##} {Unidad}".Trim()
            : string.Empty;

        public bool HayDosis => Dosis.HasValue;
    }
}
