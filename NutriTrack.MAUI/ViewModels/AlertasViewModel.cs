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
        private readonly IAlertasLeidasService _leidasService;

        public AlertasViewModel(IAlertaService alertaService, IAlertasLeidasService leidasService)
        {
            _alertaService = alertaService;
            _leidasService = leidasService;
        }

        // Filas nuevas en cada carga, con el estado leída guardado en el dispositivo
        public ObservableCollection<AlertaFila> Alertas { get; } = [];

        // Mensaje cuando no hay alertas en el rango del back
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        // Habilita "Marcar todas como leídas"
        public bool HayNoLeidas => Alertas.Any(f => !f.Leida);

        [RelayCommand]
        private async Task CargarAsync()
        {
            await EjecutarAsync(async () =>
            {
                var resultado = await _alertaService.ListarAsync();

                Alertas.Clear();

                // Sin respuesta no se sabe qué alertas existen: no se lee ni se limpia lo guardado
                if (!resultado.Exito)
                {
                    MensajeError = resultado.MensajeError;
                    MensajeVacio = null;
                    return;
                }

                var alertas = resultado.Datos?.Alertas ?? [];
                var leidas = _leidasService.ObtenerLeidas(alertas);

                // Ya llegan ordenadas por el back
                foreach (var alerta in alertas)
                {
                    var clave = _leidasService.Clave(alerta);
                    Alertas.Add(new AlertaFila(alerta, clave, leidas.Contains(clave)));
                }

                MensajeVacio = resultado.Datos?.TextoSinAlertas;
            });

            MarcarTodasCommand.NotifyCanExecuteChanged();
        }

        // Marca o desmarca una alerta. Queda en su lugar: no se reordena
        [RelayCommand]
        private void AlternarLeida(AlertaFila fila)
        {
            fila.Leida = !fila.Leida;
            GuardarLeidas();
        }

        [RelayCommand(CanExecute = nameof(HayNoLeidas))]
        private void MarcarTodas()
        {
            foreach (var fila in Alertas)
                fila.Leida = true;

            GuardarLeidas();
        }

        // La lista tiene todas las alertas vigentes: se guarda exactamente el conjunto de leídas
        private void GuardarLeidas()
        {
            _leidasService.GuardarLeidas(Alertas.Where(f => f.Leida).Select(f => f.Clave));
            MarcarTodasCommand.NotifyCanExecuteChanged();
        }
    }
}
