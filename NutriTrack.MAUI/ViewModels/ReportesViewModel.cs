using CommunityToolkit.Mvvm.Input;

namespace NutriTrack.MAUI.ViewModels
{
    // Menú de reportes (solo Administrador): Inventario (CU17), Fechas importantes (CU18)
    // y Evolución de peso (CU19). Consumo de alimentos queda fuera de alcance
    public partial class ReportesViewModel : BaseViewModel
    {
        [RelayCommand]
        private Task Inventario() => Shell.Current.GoToAsync(ReporteInventarioViewModel.Ruta);

        // TODO #117: navegar al reporte de fechas importantes
        [RelayCommand]
        private Task FechasImportantes() => MostrarEnConstruccion("Fechas importantes");

        // TODO #118: navegar al reporte de evolución de peso
        [RelayCommand]
        private Task EvolucionPeso() => MostrarEnConstruccion("Evolución de peso");

        private static Task MostrarEnConstruccion(string reporte) =>
            Shell.Current.DisplayAlertAsync(reporte, "Pantalla en construcción.", "Aceptar");
    }
}
