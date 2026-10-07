using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class AgregarAnimalPage : ContentPage
    {
        public AgregarAnimalPage(AgregarAnimalViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
