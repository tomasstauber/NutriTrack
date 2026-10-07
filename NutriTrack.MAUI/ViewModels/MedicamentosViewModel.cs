using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU29 - Consultar medicamentos
    public partial class MedicamentosViewModel : BaseViewModel
    {
        private readonly IMedicamentoService _medicamentoService;

        public MedicamentosViewModel(IMedicamentoService medicamentoService, ISesionService sesionService)
        {
            _medicamentoService = medicamentoService;

            var rol = sesionService.SesionActual?.Rol;
            EsAdministrador = rol == RolUsuario.Administrador;
            PuedeCrear = rol is RolUsuario.Administrador or RolUsuario.AsesorTecnico;
        }

        public ObservableCollection<Medicamento> Medicamentos { get; } = [];

        // R3: "Incluir inactivos" solo lo ve el Administrador
        public bool EsAdministrador { get; }

        // CU28: el Encargado solo consulta, no ve "Nuevo medicamento"
        public bool PuedeCrear { get; }

        [ObservableProperty]
        public partial string? TextoBusqueda { get; set; }

        [ObservableProperty]
        public partial bool IncluirInactivos { get; set; }

        // Mensaje cuando la lista queda vacía (E1 o E2)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        // Búsqueda en tiempo real: espera a que se deje de escribir para no
        // mandar un pedido por cada tecla
        private const int DemoraBusquedaMs = 400;
        private CancellationTokenSource? _demoraBusqueda;

        partial void OnTextoBusquedaChanged(string? value) => _ = BuscarConDemoraAsync();

        partial void OnIncluirInactivosChanged(bool value) => CargarCommand.Execute(null);

        private async Task BuscarConDemoraAsync()
        {
            // Cada tecla cancela la espera anterior: solo busca el último texto
            _demoraBusqueda?.Cancel();
            var demora = _demoraBusqueda = new CancellationTokenSource();

            try
            {
                await Task.Delay(DemoraBusquedaMs, demora.Token);
            }
            catch (TaskCanceledException)
            {
                return;
            }

            await CargarCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task CargarAsync()
        {
            string? busqueda;
            bool incluirInactivos;

            // Si mientras se esperaba la respuesta cambió el texto o el filtro
            // (EjecutarAsync ignora los pedidos que llegan ocupado), se vuelve a cargar
            // para que la lista siempre corresponda a lo que está escrito
            do
            {
                busqueda = TextoBusqueda?.Trim();
                // Los otros roles nunca mandan incluirInactivos: el back les responde 403
                incluirInactivos = EsAdministrador && IncluirInactivos;

                await ListarAsync(busqueda, incluirInactivos);
            }
            while (busqueda != TextoBusqueda?.Trim() || incluirInactivos != (EsAdministrador && IncluirInactivos));
        }

        private Task ListarAsync(string? busqueda, bool incluirInactivos) => EjecutarAsync(async () =>
        {
            var resultado = await _medicamentoService.ListarAsync(busqueda, incluirInactivos);

            Medicamentos.Clear();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                MensajeVacio = null;
                return;
            }

            // Ya llegan ordenados por nombre
            foreach (var medicamento in resultado.Datos ?? [])
                Medicamentos.Add(medicamento);

            MensajeVacio = string.IsNullOrEmpty(busqueda)
                ? "No hay medicamentos almacenados."
                : "No existe un medicamento con ese nombre.";
        });

        [RelayCommand]
        private Task NuevoMedicamento() =>
            Shell.Current.GoToAsync(NuevoMedicamentoViewModel.Ruta);
    }
}
