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

        // Al borrar la búsqueda se vuelve a mostrar el catálogo completo
        partial void OnTextoBusquedaChanged(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                CargarCommand.Execute(null);
        }

        partial void OnIncluirInactivosChanged(bool value) => CargarCommand.Execute(null);

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var busqueda = TextoBusqueda?.Trim();

            // Los otros roles nunca mandan incluirInactivos: el back les responde 403
            var resultado = await _medicamentoService.ListarAsync(busqueda, EsAdministrador && IncluirInactivos);

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
