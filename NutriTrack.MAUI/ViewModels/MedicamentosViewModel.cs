using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU29 - Consultar medicamentos, CU30 - Editar, CU31 - Desactivar y CU32 - Reactivar
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

        // CU28, CU30 y CU31: el Encargado solo consulta, no ve "Nuevo medicamento"
        // ni las acciones de las filas
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
            {
                // CU30 R1: editar y desactivar solo en filas activas; CU32: reactivar en las inactivas
                medicamento.PuedeEditarYDesactivar = PuedeCrear && medicamento.Activo;
                medicamento.PuedeReactivar = EsAdministrador && !medicamento.Activo;
                Medicamentos.Add(medicamento);
            }

            MensajeVacio = string.IsNullOrEmpty(busqueda)
                ? "No hay medicamentos almacenados."
                : "No existe un medicamento con ese nombre.";
        });

        [RelayCommand]
        private Task NuevoMedicamento() =>
            Shell.Current.GoToAsync(NuevoMedicamentoViewModel.Ruta);

        // CU30: el formulario del alta en modo edición, precargado con la fila
        [RelayCommand]
        private Task Editar(Medicamento medicamento) =>
            Shell.Current.GoToAsync(NuevoMedicamentoViewModel.Ruta, new Dictionary<string, object>
            {
                ["medicamento"] = medicamento
            });

        // CU31 - Desactivar medicamento (baja lógica)
        [RelayCommand]
        private async Task DesactivarAsync(Medicamento medicamento)
        {
            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Desactivar medicamento",
                $"¿Desea desactivar el medicamento \"{medicamento.Nombre}\"?",
                "Desactivar", "Cancelar");

            // Si cancela, no se llama a la API
            if (!confirmado)
                return;

            var desactivado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _medicamentoService.DesactivarAsync(medicamento.Id);

                if (resultado.Exito)
                    desactivado = true;
                // E3: ya estaba desactivado
                else if (resultado.CodigoEstado == HttpStatusCode.Conflict)
                    MensajeError = "El medicamento ya se encuentra desactivado";
                else
                    MensajeError = resultado.MensajeError;
            });

            if (!desactivado)
                return;

            await Shell.Current.DisplayAlertAsync("Desactivar medicamento", "Medicamento desactivado con éxito.", "Aceptar");
            await CargarAsync();
        }

        // CU32 - Reactivar medicamento (solo Administrador, filas inactivas)
        [RelayCommand]
        private async Task ReactivarAsync(Medicamento medicamento)
        {
            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Reactivar medicamento",
                $"¿Desea reactivar el medicamento \"{medicamento.Nombre}\"?",
                "Reactivar", "Cancelar");

            // Si cancela, no se llama a la API
            if (!confirmado)
                return;

            var reactivado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _medicamentoService.ReactivarAsync(medicamento.Id);

                if (resultado.Exito)
                    reactivado = true;
                // E2: ya estaba activo
                else if (resultado.CodigoEstado == HttpStatusCode.Conflict)
                    MensajeError = "El medicamento ya se encuentra activo";
                else
                    MensajeError = resultado.MensajeError;
            });

            if (!reactivado)
                return;

            await Shell.Current.DisplayAlertAsync("Reactivar medicamento", "El medicamento fue reactivado correctamente.", "Aceptar");

            // Al recargar, la fila pasa a Activo
            await CargarAsync();
        }
    }
}
