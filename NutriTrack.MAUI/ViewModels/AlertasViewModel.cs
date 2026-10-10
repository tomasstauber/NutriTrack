using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Lista de alertas vigentes (CU15 y CU16)
    // Se llega solo desde la tarjeta de alertas del panel (solo Administrador)
    public partial class AlertasViewModel : BaseViewModel
    {
        // Ruta de Shell de la lista (se registra en AppShell.xaml.cs)
        public const string Ruta = "alertas";

        private readonly IAlertaService _alertaService;

        public AlertasViewModel(IAlertaService alertaService)
        {
            _alertaService = alertaService;
        }

        public ObservableCollection<Alerta> Alertas { get; } = [];

        // Mensaje cuando no hay alertas en el rango del back
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _alertaService.ListarAsync();

            Alertas.Clear();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                MensajeVacio = null;
                return;
            }

            // Ya llegan ordenadas por el back
            foreach (var alerta in resultado.Datos?.Alertas ?? [])
                Alertas.Add(alerta);

            MensajeVacio = resultado.Datos?.TextoSinAlertas;
        });
    }
}
