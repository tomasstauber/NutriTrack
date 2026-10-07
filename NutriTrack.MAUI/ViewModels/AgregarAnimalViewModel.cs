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
        public IReadOnlyList<string> Sexos { get; } = AnimalValidador.Sexos;

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
            var cuig = AnimalValidador.NormalizarCaravana(Cuig);
            var nroManejo = AnimalValidador.NormalizarCaravana(NroManejo);
            var cuigMadre = AnimalValidador.NormalizarCaravana(CuigMadre);
            var nroManejoMadre = AnimalValidador.NormalizarCaravana(NroManejoMadre);
            var cuigPadre = AnimalValidador.NormalizarCaravana(CuigPadre);
            var nroManejoPadre = AnimalValidador.NormalizarCaravana(NroManejoPadre);
            var raza = Raza?.Trim();
            var colorPelaje = ColorPelaje?.Trim();

            // E2: se validan todos los campos a la vez, para marcar cada uno con su error.
            // Las reglas compartidas con la edición están en AnimalValidador

            // R1: validador compartido de caravana
            ErrorCaravana = CaravanaValidador.EsValida(cuig, nroManejo)
                ? null
                : CaravanaValidador.MensajeFormatoInvalido;

            // R2: fecha no posterior a hoy
            var fecha = FechaNacimiento?.Date;
            ErrorFechaNacimiento = AnimalValidador.ValidarFechaNacimiento(fecha);

            // R3: mayor a 0 y hasta 100 kg (acepta coma o punto decimal)
            ErrorPesoAlNacer = AnimalValidador.ValidarPesoAlNacer(PesoAlNacer, out var pesoKg);

            ErrorSexo = AnimalValidador.ValidarSexo(Sexo);

            ErrorRaza = AnimalValidador.ValidarRaza(raza);

            // R4: madre y padre completos o vacíos, con formato válido,
            // distintos del animal nuevo y distintos entre sí
            var madreInformada = AnimalValidador.ProgenitorInformado(cuigMadre, nroManejoMadre);
            var padreInformado = AnimalValidador.ProgenitorInformado(cuigPadre, nroManejoPadre);

            AnimalValidador.ValidarProgenitores(cuig, nroManejo,
                cuigMadre, nroManejoMadre, cuigPadre, nroManejoPadre,
                out var errorMadre, out var errorPadre);
            ErrorMadre = errorMadre;
            ErrorPadre = errorPadre;

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

            // Perfil del animal nuevo: "../" reemplaza el formulario,
            // así "volver" desde el perfil lleva a la lista (que se recarga)
            if (creado?.Id is int id)
            {
                await Shell.Current.GoToAsync($"../{PerfilAnimalViewModel.Ruta}", new Dictionary<string, object>
                {
                    ["id"] = id,
                    ["caravanaCuig"] = creado.CaravanaCuig,
                    ["caravanaNroManejo"] = creado.CaravanaNroManejo
                });
                return;
            }

            // Si la respuesta no trae id, se vuelve a la lista, que se recarga y muestra el animal nuevo
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
