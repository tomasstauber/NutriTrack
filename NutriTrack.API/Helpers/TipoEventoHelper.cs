using NutriTrack.Core.Entities.Enums;

namespace NutriTrack.API.Helpers
{
    // Texto con tilde del enum TipoEvento, para lo que ve el usuario (PDF y alertas).
    // El campo tipoEvento del JSON sigue con el valor del enum: el front lo traduce por su cuenta
    public static class TipoEventoHelper
    {
        private static readonly Dictionary<string, string> Textos = new()
        {
            [nameof(TipoEvento.Vacunacion)] = "Vacunación",
            [nameof(TipoEvento.Desparasitacion)] = "Desparasitación",
            [nameof(TipoEvento.Tratamiento)] = "Tratamiento",
            [nameof(TipoEvento.Refuerzo)] = "Refuerzo",
            [nameof(TipoEvento.Control)] = "Control",
            [nameof(TipoEvento.Otro)] = "Otro"
        };

        // Un valor desconocido se devuelve tal cual
        public static string Texto(string? valor)
        {
            return valor != null && Textos.TryGetValue(valor, out var texto)
                ? texto
                : valor ?? string.Empty;
        }
    }
}
