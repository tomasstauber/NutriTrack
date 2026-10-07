using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class UsuariosPage : ContentPage
    {
        private readonly UsuariosViewModel _viewModel;

        public UsuariosPage(UsuariosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se recarga cada vez que se entra, para ver los cambios hechos en otras pantallas
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
