namespace NutriTrack.MAUI.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        // Tambien tipo provisorio despues en la Fase 1 esto lo reemplaza el LoginViewModel
        private async void OnEntrarClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//panel");
        }
    }
}