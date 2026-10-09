using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class ReporteFechasImportantesPage : ContentPage
    {
        private readonly ReporteFechasImportantesViewModel _viewModel;

        public ReporteFechasImportantesPage(ReporteFechasImportantesViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = _viewModel = viewModel;
        }

        // Carga los rodeos del filtro al entrar y, la primera vez, genera el reporte de los próximos 7 días
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.CargarCommand.ExecuteAsync(null);
        }
    }
}
