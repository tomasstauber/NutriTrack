using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU3 - Edición de ficha individual de un animal
    // Se llega desde "Editar" del perfil, visible solo para Encargado y Administrador.
    // Recibe por navegación "caravanaCuig" y "caravanaNroManejo" y precarga la ficha actual
    public partial class EditarAnimalViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "editarAnimal";

        private readonly IAnimalService _animalService;

        // Caravana recibida por navegación; después de cargar, la de la ficha (como está guardada)
        private string? _cuig;
        private string? _nroManejo;

        public EditarAnimalViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        // true solo si la ficha se cargó bien: sin ficha no se puede guardar,
        // porque un formulario vacío borraría los datos del animal
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PuedeGuardar))]
        [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
        public partial bool FichaCargada { get; set; }

        // R6: caravana y fecha de alta se muestran, pero no se editan ni se envían

        [ObservableProperty]
        public partial string? Caravana { get; set; }

        [ObservableProperty]
        public partial DateTime FechaAlta { get; set; }

        // Datos editables

        [ObservableProperty]
        public partial DateTime? FechaNacimiento { get; set; }

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

        public IReadOnlyList<string> Sexos { get; } = AnimalValidador.Sexos;

        // R3: no se puede elegir una fecha posterior a hoy
        public DateTime Hoy => DateTime.Today;

        // E2: error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

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

        public bool HayErrorFechaNacimiento => !string.IsNullOrEmpty(ErrorFechaNacimiento);
        public bool HayErrorPesoAlNacer => !string.IsNullOrEmpty(ErrorPesoAlNacer);
        public bool HayErrorSexo => !string.IsNullOrEmpty(ErrorSexo);
        public bool HayErrorRaza => !string.IsNullOrEmpty(ErrorRaza);
        public bool HayErrorMadre => !string.IsNullOrEmpty(ErrorMadre);
        public bool HayErrorPadre => !string.IsNullOrEmpty(ErrorPadre);

        // Madre o padre de la ficha que no se pudieron separar en sus dos partes.
        // Aparte de ErrorMadre / ErrorPadre (que se recalculan al guardar) para que el aviso no se pierda:
        // mientras haya uno no se puede guardar, porque el progenitor viajaría en null y se borraría

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorLecturaMadre))]
        [NotifyPropertyChangedFor(nameof(PuedeGuardar))]
        [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
        public partial string? ErrorLecturaMadre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorLecturaPadre))]
        [NotifyPropertyChangedFor(nameof(PuedeGuardar))]
        [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
        public partial string? ErrorLecturaPadre { get; set; }

        public bool HayErrorLecturaMadre => !string.IsNullOrEmpty(ErrorLecturaMadre);
        public bool HayErrorLecturaPadre => !string.IsNullOrEmpty(ErrorLecturaPadre);

        public bool PuedeGuardar => FichaCargada && !HayErrorLecturaMadre && !HayErrorLecturaPadre;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("caravanaCuig", out var cuig))
                _cuig = cuig?.ToString();

            if (query.TryGetValue("caravanaNroManejo", out var nroManejo))
                _nroManejo = nroManejo?.ToString();
        }

        // Al aparecer: precarga la ficha una sola vez, para no pisar lo que ya se está editando
        [RelayCommand]
        private Task CargarAsync()
        {
            if (FichaCargada)
                return Task.CompletedTask;

            return EjecutarAsync(CargarFichaAsync);
        }

        private async Task CargarFichaAsync()
        {
            if (string.IsNullOrEmpty(_cuig) || string.IsNullOrEmpty(_nroManejo))
            {
                MensajeError = "No se indicó la caravana del animal.";
                return;
            }

            var resultado = await _animalService.ObtenerFichaAsync(_cuig, _nroManejo);

            if (!resultado.Exito || resultado.Datos is not { } ficha)
            {
                MensajeError = resultado.MensajeError ?? "No se pudo cargar la ficha del animal.";
                return;
            }

            _cuig = ficha.CaravanaCuig;
            _nroManejo = ficha.CaravanaNroManejo;

            Caravana = CaravanaValidador.Formatear(ficha.CaravanaCuig, ficha.CaravanaNroManejo);
            FechaAlta = ficha.FechaAlta;

            FechaNacimiento = ficha.FechaNacimiento.Date;
            // Sin formato "0.##": la columna no tiene escala fija y redondear cambiaría el peso al reenviarlo
            PesoAlNacer = ficha.PesoAlNacer.ToString(CultureInfo.InvariantCulture);
            Sexo = ficha.Sexo;
            Raza = ficha.Raza;
            ColorPelaje = ficha.ColorPelaje;

            // Llegan como "CUIG-NRO" o null; se separan para reenviarlos aunque no se toquen
            ErrorLecturaMadre = PrecargarProgenitor(ficha.Madre, "la madre", out var cuigMadre, out var nroManejoMadre);
            CuigMadre = cuigMadre;
            NroManejoMadre = nroManejoMadre;

            ErrorLecturaPadre = PrecargarProgenitor(ficha.Padre, "el padre", out var cuigPadre, out var nroManejoPadre);
            CuigPadre = cuigPadre;
            NroManejoPadre = nroManejoPadre;

            FichaCargada = true;
        }

        [RelayCommand(CanExecute = nameof(PuedeGuardar))]
        private async Task GuardarAsync()
        {
            // Además del botón deshabilitado: sin ficha o con un progenitor ilegible no se envía nada
            if (!PuedeGuardar || string.IsNullOrEmpty(_cuig) || string.IsNullOrEmpty(_nroManejo))
                return;

            MensajeError = null;

            // Mismo recorte y mayúsculas que el alta
            var cuigMadre = AnimalValidador.NormalizarCaravana(CuigMadre);
            var nroManejoMadre = AnimalValidador.NormalizarCaravana(NroManejoMadre);
            var cuigPadre = AnimalValidador.NormalizarCaravana(CuigPadre);
            var nroManejoPadre = AnimalValidador.NormalizarCaravana(NroManejoPadre);
            var raza = Raza?.Trim();
            var colorPelaje = ColorPelaje?.Trim();

            // E2: se validan todos los campos a la vez, con las mismas reglas que el alta (AnimalValidador)

            // R3: fecha no posterior a hoy
            var fecha = FechaNacimiento?.Date;
            ErrorFechaNacimiento = AnimalValidador.ValidarFechaNacimiento(fecha);

            // R4: mayor a 0 y hasta 100 kg (acepta coma o punto decimal)
            ErrorPesoAlNacer = AnimalValidador.ValidarPesoAlNacer(PesoAlNacer, out var pesoKg);

            ErrorSexo = AnimalValidador.ValidarSexo(Sexo);

            ErrorRaza = AnimalValidador.ValidarRaza(raza);

            // R5: madre y padre completos o vacíos, distintos entre sí y distintos del propio animal.
            // El back lo compara por id; acá se compara con la caravana de la ficha, normalizada igual
            var madreInformada = AnimalValidador.ProgenitorInformado(cuigMadre, nroManejoMadre);
            var padreInformado = AnimalValidador.ProgenitorInformado(cuigPadre, nroManejoPadre);

            AnimalValidador.ValidarProgenitores(
                AnimalValidador.NormalizarCaravana(_cuig), AnimalValidador.NormalizarCaravana(_nroManejo),
                cuigMadre, nroManejoMadre, cuigPadre, nroManejoPadre,
                out var errorMadre, out var errorPadre);
            ErrorMadre = errorMadre;
            ErrorPadre = errorPadre;

            // E2: Guardar no llama a la API mientras haya errores
            if (HayErrorFechaNacimiento || HayErrorPesoAlNacer || HayErrorSexo ||
                HayErrorRaza || HayErrorMadre || HayErrorPadre)
                return;

            var pedido = new EditarAnimalRequest
            {
                FechaNacimiento = DateOnly.FromDateTime(fecha!.Value),
                PesoAlNacer = pesoKg,
                Sexo = Sexo!,
                Raza = raza!,
                ColorPelaje = string.IsNullOrEmpty(colorPelaje) ? null : colorPelaje,
                // El PUT reemplaza todo: sin informar viajan en null y se borran
                CaravanaCuigMadre = madreInformada ? cuigMadre : null,
                CaravanaNroManejoMadre = madreInformada ? nroManejoMadre : null,
                CaravanaCuigPadre = padreInformado ? cuigPadre : null,
                CaravanaNroManejoPadre = padreInformado ? nroManejoPadre : null
            };

            var cuig = _cuig;
            var nroManejo = _nroManejo;
            var exito = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _animalService.EditarAsync(cuig, nroManejo, pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    return;
                }

                // E3 (400 / 404): se muestra el mensaje del back y el formulario conserva lo cargado
                MensajeError = resultado.MensajeError;
            });

            if (!exito)
                return;

            await Shell.Current.DisplayAlertAsync("Editar animal", "Cambios guardados", "Aceptar");

            // El perfil se recarga al aparecer y muestra los datos nuevos
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        // null: sin progenitor, campos vacíos. Texto que no se puede separar: campos vacíos y error
        private static string? PrecargarProgenitor(string? texto, string rol, out string cuig, out string nroManejo)
        {
            cuig = string.Empty;
            nroManejo = string.Empty;

            if (texto is null)
                return null;

            if (CaravanaValidador.IntentarSeparar(texto, out cuig, out nroManejo))
                return null;

            return $"No se pudo leer la caravana de {rol} de la ficha (\"{texto}\"). " +
                   "No se puede guardar: se borraría el dato.";
        }
    }
}
