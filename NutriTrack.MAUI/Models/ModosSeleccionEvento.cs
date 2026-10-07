namespace NutriTrack.MAUI.Models
{
    // Valores de modoSeleccion de POST api/EventoSanitario/multiple.
    // El back los compara con igualdad exacta: sin tilde y con estas mayúsculas.
    // Cualquier otro texto da 400. El texto que se ve en pantalla está en OpcionModoSeleccion
    public static class ModosSeleccionEvento
    {
        public const string RodeoCompleto = "Rodeo completo";
        public const string SeleccionManual = "Seleccion manual";
        public const string SeleccionLibre = "Seleccion libre";
    }
}
