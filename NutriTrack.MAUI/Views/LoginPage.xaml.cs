using NutriTrack.MAUI.ViewModels;

namespace NutriTrack.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // conecta la View con su ViewModel
        }
    }
}
