using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class NuevoPlanAlimenticioPage : ContentPage
    {
        private readonly NuevoPlanAlimenticioViewModel _viewModel;

        public NuevoPlanAlimenticioPage(NuevoPlanAlimenticioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga el catálogo de ingredientes del Picker al entrar
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
