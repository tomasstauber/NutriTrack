namespace NutriTrack.MAUI.Models
{
    // Opción del Picker de tipo de evento sanitario.
    // Texto se muestra (con tilde); Valor viaja (nombre del enum TipoEvento del back, sin tilde)
    public class OpcionTipoEvento
    {
        public string Texto { get; init; } = string.Empty;
        public string Valor { get; init; } = string.Empty;
    }
}
