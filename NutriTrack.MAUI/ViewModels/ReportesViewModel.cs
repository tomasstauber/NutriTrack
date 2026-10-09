using CommunityToolkit.Mvvm.Input;

namespace NutriTrack.MAUI.ViewModels
{
    // Menú de reportes (solo Administrador): Inventario (CU17), Fechas importantes (CU18)
    // y Evolución de peso (CU19). Consumo de alimentos queda fuera de alcance
    public partial class ReportesViewModel : BaseViewModel
    {
        [RelayCommand]
        private Task Inventario() => Shell.Current.GoToAsync(ReporteInventarioViewModel.Ruta);

        [RelayCommand]
        private Task FechasImportantes() => Shell.Current.GoToAsync(ReporteFechasImportantesViewModel.Ruta);

        [RelayCommand]
        private Task EvolucionPeso() => Shell.Current.GoToAsync(ReporteEvolucionPesoViewModel.Ruta);
    }
}
