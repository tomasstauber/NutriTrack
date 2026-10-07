using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class MedicamentosPage : ContentPage
    {
        private readonly MedicamentosViewModel _viewModel;

        public MedicamentosPage(MedicamentosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se recarga cada vez que se entra, para ver el medicamento recién creado
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
