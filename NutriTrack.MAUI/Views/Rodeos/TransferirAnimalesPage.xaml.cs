using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class TransferirAnimalesPage : ContentPage
    {
        private readonly TransferirAnimalesViewModel _viewModel;

        public TransferirAnimalesPage(TransferirAnimalesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los rodeos de los dos Picker al entrar (conserva los elegidos si se vuelve a cargar)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
