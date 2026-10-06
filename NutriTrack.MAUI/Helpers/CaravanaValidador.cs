namespace NutriTrack.MAUI.Helpers
{
    // Valida y muestra caravanas con la misma regla que el back (CaravanaHelper)
    // Cada pantalla pone sus dos Entry (CUIG y número de manejo) y llama a esta clase
    public static class CaravanaValidador
    {
        public const int LargoMaximoParte = 5;

        public const string MensajeFormatoInvalido =
            "Formato de caravana inválido (cada parte alfanumérica, hasta 5 caracteres).";

        // Obligatoria, de 1 a 5 caracteres y solo letras o números
        // char.IsLetterOrDigit acepta ñ y tildes, igual que el back
        public static bool ParteValida(string? parte)
        {
            return !string.IsNullOrEmpty(parte)
                && parte.Length <= LargoMaximoParte
                && parte.All(char.IsLetterOrDigit);
        }

        public static bool EsValida(string? cuig, string? nroManejo)
        {
            return ParteValida(cuig) && ParteValida(nroManejo);
        }

        // Formato único para mostrar una caravana: CUIG-NRO (ej. AR001-00001), igual que los reportes
        public static string Formatear(string cuig, string nroManejo)
        {
            return $"{cuig}-{nroManejo}";
        }
    }
}
