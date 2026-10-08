using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class NuevoIngredientePage : ContentPage
    {
        public NuevoIngredientePage(NuevoIngredienteViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
