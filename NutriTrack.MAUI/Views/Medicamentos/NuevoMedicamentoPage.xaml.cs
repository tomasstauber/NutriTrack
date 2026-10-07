using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class NuevoMedicamentoPage : ContentPage
    {
        public NuevoMedicamentoPage(NuevoMedicamentoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
