namespace NutriTrack.MAUI.Models
{
    // Tipos de evento sanitario (enum TipoEvento del back). Viajan sin tilde ("Vacunacion");
    // se muestran con tilde. Una sola tabla para el registro de eventos y los reportes
    public static class TiposEvento
    {
        public static IReadOnlyList<OpcionTipoEvento> Opciones { get; } =
        [
            new() { Texto = "Vacunación", Valor = "Vacunacion" },
            new() { Texto = "Desparasitación", Valor = "Desparasitacion" },
            new() { Texto = "Tratamiento", Valor = "Tratamiento" },
            new() { Texto = "Refuerzo", Valor = "Refuerzo" },
            new() { Texto = "Control", Valor = "Control" },
            new() { Texto = "Otro", Valor = "Otro" }
        ];

        // Texto con tilde del valor que manda el back. Si no lo conoce, lo devuelve tal cual
        public static string Texto(string? valor) =>
            Opciones.FirstOrDefault(o => o.Valor == valor)?.Texto ?? valor ?? string.Empty;
    }
}
