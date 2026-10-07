using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class RodeosPage : ContentPage
    {
        private readonly RodeosViewModel _viewModel;

        public RodeosPage(RodeosViewModel viewModel)
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
