namespace NutriTrack.MAUI.Models
{
    // Opción del Picker de modo de selección del evento sanitario.
    // Texto se muestra (con tilde); Valor viaja (una constante de ModosSeleccionEvento)
    public class OpcionModoSeleccion
    {
        public string Texto { get; init; } = string.Empty;
        public string Valor { get; init; } = string.Empty;
    }
}
