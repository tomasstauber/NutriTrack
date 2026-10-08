using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU28 - Crear medicamento y CU30 - Editar medicamento
    // Para editar se navega con "medicamento" (la fila de la lista): no se usa el GET por id
    public partial class NuevoMedicamentoViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "nuevo-medicamento";

        private const string MensajeDuplicado = "Ya existe un medicamento";
        private const string AclaracionDuplicado =
            "Si no aparece en la lista, puede estar inactivo: lo reactiva un Administrador.";
        private const string MensajeNoExiste = "No existe un medicamento con ese id";

        private readonly IMedicamentoService _medicamentoService;

        // Id del medicamento que se edita; null en el alta
        private int? _idMedicamento;

        public NuevoMedicamentoViewModel(IMedicamentoService medicamentoService)
        {
            _medicamentoService = medicamentoService;
        }

        [ObservableProperty]
        public partial string Titulo { get; set; } = "Nuevo medicamento";

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Descripcion { get; set; }

        // CU30: precarga con los datos de la fila
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("medicamento", out var valor) || valor is not Medicamento medicamento)
                return;

            _idMedicamento = medicamento.Id;
            Titulo = "Editar medicamento";
            Nombre = medicamento.Nombre;
            Descripcion = medicamento.Descripcion;
        }

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

            var guardado = _idMedicamento is int idMedicamento
                ? await EditarAsync(idMedicamento, pedido)
                : await CrearAsync(pedido);

            if (!guardado)
                return;

            // La lista se recarga al volver
            await Shell.Current.GoToAsync("..");
        }

        private async Task<bool> CrearAsync(MedicamentoRequest pedido)
        {
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

            if (creado)
                await Shell.Current.DisplayAlertAsync("Nuevo medicamento", "Medicamento creado con éxito.", "Aceptar");

            return creado;
        }

        private async Task<bool> EditarAsync(int idMedicamento, MedicamentoRequest pedido)
        {
            var editado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _medicamentoService.EditarAsync(idMedicamento, pedido);

                if (resultado.Exito)
                {
                    editado = true;
                    return;
                }

                // CU30 E1: no existe (o lo desactivaron mientras tanto).
                // E2: nombre repetido, con el mensaje del back
                MensajeError = resultado.CodigoEstado == HttpStatusCode.NotFound
                    ? MensajeNoExiste
                    : resultado.MensajeError;
            });

            if (editado)
                await Shell.Current.DisplayAlertAsync("Editar medicamento", "Medicamento actualizado con éxito.", "Aceptar");

            return editado;
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
