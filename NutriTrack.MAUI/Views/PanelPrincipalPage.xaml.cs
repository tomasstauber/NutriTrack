namespace NutriTrack.MAUI.Views
{
    public partial class PanelPrincipalPage : ContentPage
    {
        public PanelPrincipalPage()
        {
            InitializeComponent();
        }

        // Provosira en la Fase 1 esto también va a cerrar la sesion real
        private async void OnCerrarSesionClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}