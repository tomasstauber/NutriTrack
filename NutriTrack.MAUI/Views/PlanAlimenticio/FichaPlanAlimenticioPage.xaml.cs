using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class FichaPlanAlimenticioPage : ContentPage
    {
        private readonly FichaPlanAlimenticioViewModel _viewModel;

        public FichaPlanAlimenticioPage(FichaPlanAlimenticioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Se recarga cada vez que se entra: al volver de asignar o editar se ven los cambios
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
