using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class AlertasPage : ContentPage
    {
        private readonly AlertasViewModel _viewModel;

        public AlertasPage(AlertasViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se pide de nuevo cada vez que se entra, para que la lista esté al día
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
