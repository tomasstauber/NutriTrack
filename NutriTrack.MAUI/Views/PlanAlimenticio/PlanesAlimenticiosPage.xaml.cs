using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class PlanesAlimenticiosPage : ContentPage
    {
        private readonly PlanesAlimenticiosViewModel _viewModel;

        public PlanesAlimenticiosPage(PlanesAlimenticiosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se recarga cada vez que se entra, para ver el plan recién creado
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
