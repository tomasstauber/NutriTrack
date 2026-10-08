namespace NutriTrack.MAUI.Views
{
    // Filtro de período de los reportes (#116). El BindingContext lo pone la pantalla
    // del reporte con su FiltroPeriodoReporteViewModel, por eso acá no se inyecta nada
    public partial class FiltroPeriodoReporteView : ContentView
    {
        public FiltroPeriodoReporteView()
        {
            InitializeComponent();
        }
    }
}
