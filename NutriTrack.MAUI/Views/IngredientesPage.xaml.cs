using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class IngredientesPage : ContentPage
    {
        private readonly IngredientesViewModel _viewModel;

        public IngredientesPage(IngredientesViewModel viewModel)
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
