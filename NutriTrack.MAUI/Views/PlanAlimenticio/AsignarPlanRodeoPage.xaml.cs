using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class AsignarPlanRodeoPage : ContentPage
    {
        private readonly AsignarPlanRodeoViewModel _viewModel;

        public AsignarPlanRodeoPage(AsignarPlanRodeoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los rodeos del Picker al entrar (conserva el elegido si se vuelve a cargar)
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
