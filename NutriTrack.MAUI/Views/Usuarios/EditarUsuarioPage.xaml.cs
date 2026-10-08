using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class EditarUsuarioPage : ContentPage
    {
        public EditarUsuarioPage(EditarUsuarioViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
