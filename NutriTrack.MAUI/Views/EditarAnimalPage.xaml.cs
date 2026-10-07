using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class EditarAnimalPage : ContentPage
    {
        private readonly EditarAnimalViewModel _viewModel;

        public EditarAnimalPage(EditarAnimalViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Precarga la ficha al aparecer (el ViewModel la pide una sola vez)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
