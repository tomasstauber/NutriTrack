using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class PerfilAnimalPage : ContentPage
    {
        private readonly PerfilAnimalViewModel _viewModel;

        public PerfilAnimalPage(PerfilAnimalViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se recarga cada vez que se entra: al volver de Registrar peso se ve el último peso nuevo
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
