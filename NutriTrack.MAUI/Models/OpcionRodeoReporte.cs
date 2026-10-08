namespace NutriTrack.MAUI.Models
{
    // Opción del Picker de rodeo de los reportes.
    // "Todos" tiene IdRodeo null: no se manda el parámetro
    public class OpcionRodeoReporte
    {
        public string Texto { get; init; } = string.Empty;
        public int? IdRodeo { get; init; }
    }
}
