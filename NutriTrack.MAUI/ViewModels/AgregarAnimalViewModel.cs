using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU1 - Alta de nuevo animal (con madre y padre)
    // Se llega desde "Agregar animal" de la lista, visible solo para Encargado y Administrador
    public partial class AgregarAnimalViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "agregarAnimal";

        private const decimal PesoMaximoKg = 100;

        private readonly IAnimalService _animalService;

        public AgregarAnimalViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        // Datos del animal

        [ObservableProperty]
        public partial string? Cuig { get; set; }

        [ObservableProperty]
        public partial string? NroManejo { get; set; }

        [ObservableProperty]
        public partial DateTime? FechaNacimiento { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial string? PesoAlNacer { get; set; }

        [ObservableProperty]
        public partial string? Sexo { get; set; }

        [ObservableProperty]
        public partial string? Raza { get; set; }

        [ObservableProperty]
        public partial string? ColorPelaje { get; set; }

        // Madre y padre (opcionales: las dos partes o ninguna)

        [ObservableProperty]
        public partial string? CuigMadre { get; set; }

        [ObservableProperty]
        public partial string? NroManejoMadre { get; set; }

        [ObservableProperty]
        public partial string? CuigPadre { get; set; }

        [ObservableProperty]
        public partial string? NroManejoPadre { get; set; }

        // Opciones del Picker: viajan tal cual como texto
        public IReadOnlyList<string> Sexos { get; } = ["Macho", "Hembra"];

        // R2: no se puede elegir una fecha posterior a hoy
        public DateTime Hoy => DateTime.Today;

        // E2: error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorCaravana))]
        public partial string? ErrorCaravana { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorFechaNacimiento))]
        public partial string? ErrorFechaNacimiento { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorPesoAlNacer))]
        public partial string? ErrorPesoAlNacer { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorSexo))]
        public partial string? ErrorSexo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorRaza))]
        public partial string? ErrorRaza { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorMadre))]
        public partial string? ErrorMadre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorPadre))]
        public partial string? ErrorPadre { get; set; }

        public bool HayErrorCaravana => !string.IsNullOrEmpty(ErrorCaravana);
        public bool HayErrorFechaNacimiento => !string.IsNullOrEmpty(ErrorFechaNacimiento);
        public bool HayErrorPesoAlNacer => !string.IsNullOrEmpty(ErrorPesoAlNacer);
        public bool HayErrorSexo => !string.IsNullOrEmpty(ErrorSexo);
        public bool HayErrorRaza => !string.IsNullOrEmpty(ErrorRaza);
        public bool HayErrorMadre => !string.IsNullOrEmpty(ErrorMadre);
        public bool HayErrorPadre => !string.IsNullOrEmpty(ErrorPadre);

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            // El back guarda la caravana tal cual llega y distingue mayúsculas:
            // se recorta y se pasa a mayúsculas antes de validar y enviar
            var cuig = NormalizarCaravana(Cuig);
            var nroManejo = NormalizarCaravana(NroManejo);
            var cuigMadre = NormalizarCaravana(CuigMadre);
            var nroManejoMadre = NormalizarCaravana(NroManejoMadre);
            var cuigPadre = NormalizarCaravana(CuigPadre);
            var nroManejoPadre = NormalizarCaravana(NroManejoPadre);
            var raza = Raza?.Trim();
            var colorPelaje = ColorPelaje?.Trim();

            // E2: se validan todos los campos a la vez, para marcar cada uno con su error

            // R1: validador compartido de caravana
            ErrorCaravana = CaravanaValidador.EsValida(cuig, nroManejo)
                ? null
                : CaravanaValidador.MensajeFormatoInvalido;

            // R2: fecha no posterior a hoy
            var fecha = FechaNacimiento?.Date;
            ErrorFechaNacimiento = fecha switch
            {
                null => "La fecha de nacimiento es obligatoria.",
                _ when fecha > DateTime.Today => "La fecha de nacimiento no puede ser posterior a hoy.",
                _ => null
            };

            // R3: mayor a 0 y hasta 100 kg (acepta coma o punto decimal)
            var pesoValido = IntentarLeerPeso(PesoAlNacer, out var pesoKg) && pesoKg > 0 && pesoKg <= PesoMaximoKg;
            ErrorPesoAlNacer = pesoValido
                ? null
                : "El peso al nacer debe ser mayor a 0 y menor o igual a 100 kg.";

            ErrorSexo = Sexos.Contains(Sexo) ? null : "El sexo es obligatorio.";

            ErrorRaza = string.IsNullOrEmpty(raza) ? "La raza es obligatoria." : null;

            // R4: madre y padre completos o vacíos, con formato válido,
            // distintos del animal nuevo y distintos entre sí
            var madreInformada = ProgenitorInformado(cuigMadre, nroManejoMadre);
            var padreInformado = ProgenitorInformado(cuigPadre, nroManejoPadre);

            ErrorMadre = null;
            if (madreInformada)
            {
                ErrorMadre = ValidarProgenitor(cuigMadre, nroManejoMadre, "la madre");
                if (ErrorMadre is null && cuigMadre == cuig && nroManejoMadre == nroManejo)
                    ErrorMadre = "El animal no puede ser su propia madre.";
            }

            ErrorPadre = null;
            if (padreInformado)
            {
                ErrorPadre = ValidarProgenitor(cuigPadre, nroManejoPadre, "el padre");
                if (ErrorPadre is null && cuigPadre == cuig && nroManejoPadre == nroManejo)
                    ErrorPadre = "El animal no puede ser su propio padre.";
            }

            if (madreInformada && padreInformado && ErrorMadre is null && ErrorPadre is null &&
                cuigMadre == cuigPadre && nroManejoMadre == nroManejoPadre)
                ErrorPadre = "La madre y el padre no pueden ser el mismo animal.";

            // E2: Guardar no llama a la API mientras haya errores
            if (HayErrorCaravana || HayErrorFechaNacimiento || HayErrorPesoAlNacer ||
                HayErrorSexo || HayErrorRaza || HayErrorMadre || HayErrorPadre)
                return;

            var pedido = new CrearAnimalRequest
            {
                CaravanaCuig = cuig,
                CaravanaNroManejo = nroManejo,
                FechaNacimiento = DateOnly.FromDateTime(fecha!.Value),
                PesoAlNacer = pesoKg,
                // Sin informar viajan en null, no como texto vacío
                CaravanaCuigMadre = madreInformada ? cuigMadre : null,
                CaravanaNroManejoMadre = madreInformada ? nroManejoMadre : null,
                CaravanaCuigPadre = padreInformado ? cuigPadre : null,
                CaravanaNroManejoPadre = padreInformado ? nroManejoPadre : null,
                Raza = raza!,
                Sexo = Sexo!,
                ColorPelaje = string.IsNullOrEmpty(colorPelaje) ? null : colorPelaje
            };

            AnimalCreado? creado = null;
            var exito = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _animalService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    creado = resultado.Datos;
                    return;
                }

                // E1 (409) y E3 (400): se muestra el mensaje del back y el formulario conserva lo cargado
                MensajeError = resultado.MensajeError;
            });

            if (!exito)
                return;

            await Shell.Current.DisplayAlertAsync("Agregar animal", "Alta exitosa", "Aceptar");

            // TODO #104: cuando exista el perfil, ir al perfil del animal nuevo en lugar de la lista
            // (reemplaza el formulario, así "volver" desde el perfil lleva a la lista).
            // Si la respuesta no trae id, se sigue volviendo a la lista:
            // if (creado?.Id is int id)
            // {
            //     await Shell.Current.GoToAsync("../perfilAnimal", new Dictionary<string, object>
            //     {
            //         ["id"] = id,
            //         ["caravanaCuig"] = creado.CaravanaCuig,
            //         ["caravanaNroManejo"] = creado.CaravanaNroManejo
            //     });
            //     return;
            // }

            // La lista se recarga al volver y muestra el animal nuevo
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        // Recorta los espacios y pasa a mayúsculas ("  ar001" -> "AR001")
        private static string NormalizarCaravana(string? parte) =>
            parte?.Trim().ToUpperInvariant() ?? string.Empty;

        // Misma regla que el back (CaravanaHelper.ProgenitorInformado), pero ya recortado:
        // un espacio en blanco no cuenta como informado
        private static bool ProgenitorInformado(string cuig, string nroManejo) =>
            !string.IsNullOrEmpty(cuig) || !string.IsNullOrEmpty(nroManejo);

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
