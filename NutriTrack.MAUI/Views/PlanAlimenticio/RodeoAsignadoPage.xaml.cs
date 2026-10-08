using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class RodeoAsignadoPage : ContentPage
    {
        private readonly RodeoAsignadoViewModel _viewModel;

        public RodeoAsignadoPage(RodeoAsignadoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga la primera página de animales del rodeo al entrar
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
