using System.Globalization;

namespace NutriTrack.MAUI.Helpers
{
    // Validaciones de la ficha de un animal, compartidas por el alta y la edición.
    // Cada función devuelve el mensaje de error del campo, o null si está bien
    public static class AnimalValidador
    {
        public const decimal PesoMaximoKg = 100;

        // Opciones del Picker: viajan tal cual como texto
        public static IReadOnlyList<string> Sexos { get; } = ["Macho", "Hembra"];

        // El back guarda la caravana tal cual llega y distingue mayúsculas:
        // se recorta y se pasa a mayúsculas antes de validar y enviar ("  ar001" -> "AR001")
        public static string NormalizarCaravana(string? parte) =>
            parte?.Trim().ToUpperInvariant() ?? string.Empty;

        // Fecha obligatoria y no posterior a hoy
        public static string? ValidarFechaNacimiento(DateTime? fecha) => fecha?.Date switch
        {
            null => "La fecha de nacimiento es obligatoria.",
            var dia when dia > DateTime.Today => "La fecha de nacimiento no puede ser posterior a hoy.",
            _ => null
        };

        // Mayor a 0 y hasta 100 kg (acepta coma o punto decimal)
        public static string? ValidarPesoAlNacer(string? texto, out decimal pesoKg)
        {
            var pesoValido = IntentarLeerPeso(texto, out pesoKg) && pesoKg > 0 && pesoKg <= PesoMaximoKg;
            return pesoValido
                ? null
                : "El peso al nacer debe ser mayor a 0 y menor o igual a 100 kg.";
        }

        public static string? ValidarSexo(string? sexo) =>
            Sexos.Contains(sexo) ? null : "El sexo es obligatorio.";

        // Recibe la raza ya recortada
        public static string? ValidarRaza(string? raza) =>
            string.IsNullOrEmpty(raza) ? "La raza es obligatoria." : null;

        // Misma regla que el back (CaravanaHelper.ProgenitorInformado), pero ya recortado:
        // un espacio en blanco no cuenta como informado
        public static bool ProgenitorInformado(string cuig, string nroManejo) =>
            !string.IsNullOrEmpty(cuig) || !string.IsNullOrEmpty(nroManejo);

        // Madre y padre completos o vacíos, con formato válido,
        // distintos del propio animal y distintos entre sí.
        // Recibe todas las caravanas ya normalizadas
        public static void ValidarProgenitores(string cuig, string nroManejo,
            string cuigMadre, string nroManejoMadre, string cuigPadre, string nroManejoPadre,
            out string? errorMadre, out string? errorPadre)
        {
            var madreInformada = ProgenitorInformado(cuigMadre, nroManejoMadre);
            var padreInformado = ProgenitorInformado(cuigPadre, nroManejoPadre);

            errorMadre = null;
            if (madreInformada)
            {
                errorMadre = ValidarProgenitor(cuigMadre, nroManejoMadre, "la madre");
                if (errorMadre is null && cuigMadre == cuig && nroManejoMadre == nroManejo)
                    errorMadre = "El animal no puede ser su propia madre.";
            }

            errorPadre = null;
            if (padreInformado)
            {
                errorPadre = ValidarProgenitor(cuigPadre, nroManejoPadre, "el padre");
                if (errorPadre is null && cuigPadre == cuig && nroManejoPadre == nroManejo)
                    errorPadre = "El animal no puede ser su propio padre.";
            }

            if (madreInformada && padreInformado && errorMadre is null && errorPadre is null &&
                cuigMadre == cuigPadre && nroManejoMadre == nroManejoPadre)
                errorPadre = "La madre y el padre no pueden ser el mismo animal.";
        }

        // Mismos mensajes que el back (CaravanaHelper.ErrorProgenitor)
        private static string? ValidarProgenitor(string cuig, string nroManejo, string rol)
        {
            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(nroManejo))
                return $"La caravana de {rol} está incompleta: informe el CUIG y el número de manejo.";

            if (!CaravanaValidador.EsValida(cuig, nroManejo))
                return $"Formato de caravana de {rol} inválido (cada parte alfanumérica, hasta 5 caracteres).";

            return null;
        }

        // Acepta coma o punto decimal, sin depender del idioma de la computadora
        private static bool IntentarLeerPeso(string? texto, out decimal pesoKg)
        {
            var normalizado = texto?.Trim().Replace(',', '.');
            return decimal.TryParse(normalizado, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out pesoKg);
        }
    }
}
