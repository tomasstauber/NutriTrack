using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;
using NutriTrack.MAUI.ViewModels;

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
        private readonly PanelPrincipalViewModel _vm;

        public PanelPrincipalPage(PanelPrincipalViewModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm; // conecta la View con su ViewModel
        }

        // Shell reutiliza la página: se recarga cada vez que aparece
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.CargarCommand.Execute(null);
        }
    }
}
