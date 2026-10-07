using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class CrearRodeoPage : ContentPage
    {
        private readonly CrearRodeoViewModel _viewModel;

        public CrearRodeoPage(CrearRodeoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los animales sin rodeo del selector al entrar
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
