using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class RegistrarPesoPage : ContentPage
    {
        public RegistrarPesoPage(RegistrarPesoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
