using CommunityToolkit.Mvvm.ComponentModel;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.ViewModels
{
    // Fila de la lista de alertas: la alerta y si está leída.
    // Alerta queda como vino de la API (no observable); el estado de pantalla vive acá.
    // La lista se arma de nuevo en cada OnAppearing con lo guardado en el dispositivo
    public partial class AlertaFila : ObservableObject
    {
        public AlertaFila(Alerta alerta, string clave, bool leida)
        {
            Alerta = alerta;
            Clave = clave;
            Leida = leida;
        }

        public Alerta Alerta { get; }

        // Clave con la que se guarda en el dispositivo (la arma IAlertasLeidasService)
        public string Clave { get; }

        // Leída: sigue en su lugar, atenuada
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoAccion))]
        public partial bool Leida { get; set; }

        public string TextoAccion => Leida ? "Marcar como no leída" : "Marcar como leída";
    }
}
