using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class NuevoUsuarioPage : ContentPage
    {
        public NuevoUsuarioPage(NuevoUsuarioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
