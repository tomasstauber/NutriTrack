using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class ReporteInventarioPage : ContentPage
    {
        private readonly ReporteInventarioViewModel _viewModel;

        public ReporteInventarioPage(ReporteInventarioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los rodeos del filtro al entrar (conserva el elegido si se vuelve a cargar)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
