using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.Views
{
    public partial class PanelPrincipalPage : ContentPage
    {
        private readonly ISesionService _sesionService;

        public PanelPrincipalPage(ISesionService sesionService)
        {
            InitializeComponent();
            _sesionService = sesionService;
        }

        // Entradas del menú según el rol del usuario logueado
        protected override void OnAppearing()
        {
            base.OnAppearing();

            var rol = _sesionService.SesionActual?.Rol;

            // Mientras el login sea provisorio no hay sesión: se muestra para poder probar
            BotonIngredientes.IsVisible =
                rol is null or RolUsuario.AsesorTecnico or RolUsuario.Administrador;
        }

        private async void OnIngredientesClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("ingredientes");
        }

        // Provosira en la Fase 1 esto también va a cerrar la sesion real
        private async void OnCerrarSesionClicked(object? sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//login");
        }
    }
}