using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU28 - Crear medicamento
    public partial class NuevoMedicamentoViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "nuevo-medicamento";

        private const string MensajeDuplicado = "Ya existe un medicamento";
        private const string AclaracionDuplicado =
            "Si no aparece en la lista, puede estar inactivo: lo reactiva un Administrador.";

        private readonly IMedicamentoService _medicamentoService;

        public NuevoMedicamentoViewModel(IMedicamentoService medicamentoService)
        {
            _medicamentoService = medicamentoService;
        }

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Descripcion { get; set; }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            var nombre = Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MensajeError = "El nombre del medicamento es obligatorio.";
                return;
            }

            // Descripción opcional: vacía viaja en null
            var descripcion = Descripcion?.Trim();
            var pedido = new MedicamentoRequest
            {
                Nombre = nombre,
                Descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion
            };

            var creado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _medicamentoService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    creado = true;
                    return;
                }

                // E1: el duplicado cuenta también los inactivos, pero el back no dice
                // si el existente está inactivo; se suma una aclaración fija
                MensajeError = resultado.MensajeError?.StartsWith(MensajeDuplicado, StringComparison.OrdinalIgnoreCase) == true
                    ? $"{resultado.MensajeError} {AclaracionDuplicado}"
                    : resultado.MensajeError;
            });

            if (!creado)
                return;

            await Shell.Current.DisplayAlertAsync("Nuevo medicamento", "Medicamento creado con éxito.", "Aceptar");

            // La lista se recarga al volver y muestra el nuevo como Activo
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
