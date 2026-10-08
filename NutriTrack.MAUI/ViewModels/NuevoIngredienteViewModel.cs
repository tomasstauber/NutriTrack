using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU24 - Crear ingrediente
    public partial class NuevoIngredienteViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "nuevo-ingrediente";

        private readonly IIngredienteService _ingredienteService;

        public NuevoIngredienteViewModel(IIngredienteService ingredienteService)
        {
            _ingredienteService = ingredienteService;
        }

        // R3: texto exacto del enum UnidadMedida del back, en minúscula
        public IReadOnlyList<string> Unidades { get; } = ["kg", "l", "gr", "ml", "fardo", "rollo"];

        // D1 a D8

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Descripcion { get; set; }

        [ObservableProperty]
        public partial string? Minerales { get; set; }

        // Texto de los Entry: se convierten a número al guardar (acepta coma o punto)

        [ObservableProperty]
        public partial string? EnergiaMetabolizable { get; set; }

        [ObservableProperty]
        public partial string? ProteinaBruta { get; set; }

        [ObservableProperty]
        public partial string? FibraDetergenteNeutro { get; set; }

        [ObservableProperty]
        public partial string? UnidadMedida { get; set; }

        [ObservableProperty]
        public partial string? Aditivos { get; set; }

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorNombre))]
        public partial string? ErrorNombre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorEnergiaMetabolizable))]
        public partial string? ErrorEnergiaMetabolizable { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorProteinaBruta))]
        public partial string? ErrorProteinaBruta { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorFibraDetergenteNeutro))]
        public partial string? ErrorFibraDetergenteNeutro { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorUnidadMedida))]
        public partial string? ErrorUnidadMedida { get; set; }

        public bool HayErrorNombre => !string.IsNullOrEmpty(ErrorNombre);
        public bool HayErrorEnergiaMetabolizable => !string.IsNullOrEmpty(ErrorEnergiaMetabolizable);
        public bool HayErrorProteinaBruta => !string.IsNullOrEmpty(ErrorProteinaBruta);
        public bool HayErrorFibraDetergenteNeutro => !string.IsNullOrEmpty(ErrorFibraDetergenteNeutro);
        public bool HayErrorUnidadMedida => !string.IsNullOrEmpty(ErrorUnidadMedida);

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            var nombre = Nombre?.Trim();

            // Se validan todos los campos a la vez, para marcar cada uno con su error

            // R1: nombre obligatorio
            ErrorNombre = string.IsNullOrEmpty(nombre) ? "El nombre del ingrediente es obligatorio." : null;

            // R2 / E2: opcionales; si se cargan, mayores o iguales a 0
            ErrorEnergiaMetabolizable = ValidarValorNutricional(EnergiaMetabolizable, "La energía metabolizable", out var energia);
            ErrorProteinaBruta = ValidarValorNutricional(ProteinaBruta, "La proteína bruta", out var proteina);
            ErrorFibraDetergenteNeutro = ValidarValorNutricional(FibraDetergenteNeutro, "La fibra detergente neutro", out var fibra);

            // R3: unidad obligatoria
            ErrorUnidadMedida = string.IsNullOrEmpty(UnidadMedida) ? "La unidad de medida es obligatoria." : null;

            if (HayErrorNombre || HayErrorEnergiaMetabolizable || HayErrorProteinaBruta ||
                HayErrorFibraDetergenteNeutro || HayErrorUnidadMedida)
                return;

            var pedido = new IngredienteRequest
            {
                NombreIngrediente = nombre!,
                Descripcion = TextoOpcional(Descripcion),
                Minerales = TextoOpcional(Minerales),
                EnergiaMetabolizable = energia,
                ProteinaBruta = proteina,
                FibraDetergenteNeutro = fibra,
                UnidadMedida = UnidadMedida!,
                Aditivos = TextoOpcional(Aditivos)
            };

            var creado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _ingredienteService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    creado = true;
                    return;
                }

                // E1: nombre duplicado (solo cuenta los activos). Se muestra el mensaje
                // del back y el formulario conserva lo cargado
                MensajeError = resultado.MensajeError;
            });

            if (!creado)
                return;

            await Shell.Current.DisplayAlertAsync("Nuevo ingrediente", "Ingrediente creado con éxito.", "Aceptar");

            // S3: el catálogo se recarga al volver y muestra el ingrediente nuevo
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        // Vacío: sin error y viaja en null (no en 0).
        // Cargado: número mayor o igual a 0, con coma o punto como separador decimal
        private static string? ValidarValorNutricional(string? texto, string campo, out decimal? valor)
        {
            valor = null;

            var normalizado = texto?.Trim().Replace(',', '.');
            if (string.IsNullOrEmpty(normalizado))
                return null;

            // AllowLeadingSign: un negativo se lee como número y recibe el mensaje de "mayor o igual a 0"
            if (!decimal.TryParse(normalizado, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture, out var numero))
                return $"{campo} debe ser un número.";

            if (numero < 0)
                return $"{campo} debe ser mayor o igual a 0.";

            valor = numero;
            return null;
        }

        private static string? TextoOpcional(string? texto)
        {
            var recortado = texto?.Trim();
            return string.IsNullOrEmpty(recortado) ? null : recortado;
        }
    }
}
