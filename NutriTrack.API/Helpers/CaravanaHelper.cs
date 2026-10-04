namespace NutriTrack.API.Helpers
{
    public static class CaravanaHelper
    {
        public const int LargoMaximoParteCaravana = 5;
        public const string MensajeCaravanaInvalida =
            "Formato de caravana inválido (cada parte alfanumérica, hasta 5 caracteres).";

        public static bool ParteValida(string? parte)
        {
            return !string.IsNullOrEmpty(parte)
                && parte.Length <= LargoMaximoParteCaravana
                && parte.All(char.IsLetterOrDigit);
        }

        public static bool ProgenitorInformado(string? cuig, string? nroManejo)
        {
            return !string.IsNullOrEmpty(cuig) || !string.IsNullOrEmpty(nroManejo);
        }

        public static string? ErrorProgenitor(string? cuig, string? nroManejo, string rol)
        {
            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(nroManejo))
                return $"La caravana de {rol} está incompleta: informe el CUIG y el número de manejo.";

            if (!ParteValida(cuig) || !ParteValida(nroManejo))
                return $"Formato de caravana de {rol} inválido (cada parte alfanumérica, hasta 5 caracteres).";

            return null;
        }
        public static bool MismaCaravana(string? cuigA, string? nroA, string? cuigB, string? nroB)
        {
            return cuigA == cuigB && nroA == nroB;
        }
    }
}