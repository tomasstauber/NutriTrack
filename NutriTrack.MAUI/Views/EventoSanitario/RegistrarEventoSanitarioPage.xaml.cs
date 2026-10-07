using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class RegistrarEventoSanitarioPage : ContentPage
    {
        private readonly RegistrarEventoSanitarioViewModel _viewModel;

        public RegistrarEventoSanitarioPage(RegistrarEventoSanitarioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Rodeos al día (cantidad de animales incluida) cada vez que se entra
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarRodeosCommand.ExecuteAsync(null);
        }
    }
}
