using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class ReporteEvolucionPesoPage : ContentPage
    {
        private readonly ReporteEvolucionPesoViewModel _viewModel;

        public ReporteEvolucionPesoPage(ReporteEvolucionPesoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los rodeos del filtro al entrar y, la primera vez, genera el reporte
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
