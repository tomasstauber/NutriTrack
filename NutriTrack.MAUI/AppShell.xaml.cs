using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Services;
using NutriTrack.MAUI.ViewModels;
using NutriTrack.MAUI.Views;

namespace NutriTrack.MAUI
{
    public partial class AppShell : Shell
    {
        private readonly ISesionService _sesionService;

        public AppShell(ISesionService sesionService)
        {
            InitializeComponent();

            // Pantallas de los módulos: se navega con "ruta" (se apilan sobre la actual)
            // y aparecen en el menú del panel según el rol de la sesión
            MenuModulos.RegistrarRuta<IngredientesPage>(MenuModulos.Ingredientes);
            MenuModulos.RegistrarRuta<MedicamentosPage>(MenuModulos.Medicamentos);

            // Pantallas internas de los módulos (no aparecen en el menú)
            Routing.RegisterRoute(NuevoIngredienteViewModel.Ruta, typeof(NuevoIngredientePage));
            Routing.RegisterRoute(NuevoMedicamentoViewModel.Ruta, typeof(NuevoMedicamentoPage));
            MenuModulos.RegistrarRuta<AnimalesPage>(MenuModulos.Animales);
            Routing.RegisterRoute(AgregarAnimalViewModel.Ruta, typeof(AgregarAnimalPage));
            Routing.RegisterRoute(PerfilAnimalViewModel.Ruta, typeof(PerfilAnimalPage));
            Routing.RegisterRoute(EditarAnimalViewModel.Ruta, typeof(EditarAnimalPage));
            MenuModulos.RegistrarRuta<RodeosPage>(MenuModulos.Rodeos);
            Routing.RegisterRoute(CrearRodeoViewModel.Ruta, typeof(CrearRodeoPage));
            Routing.RegisterRoute(TransferirAnimalesViewModel.Ruta, typeof(TransferirAnimalesPage));
            MenuModulos.RegistrarRuta<UsuariosPage>(MenuModulos.Usuarios);
            Routing.RegisterRoute(NuevoUsuarioViewModel.Ruta, typeof(NuevoUsuarioPage));
            MenuModulos.RegistrarRuta<RegistrarPesoPage>(MenuModulos.Peso);
            MenuModulos.RegistrarRuta<RegistrarEventoSanitarioPage>(MenuModulos.EventoSanitario);
            MenuModulos.RegistrarRuta<PlanesAlimenticiosPage>(MenuModulos.PlanAlimenticio);
            MenuModulos.RegistrarRuta<ReportesPage>(MenuModulos.Reportes);
            Routing.RegisterRoute(ReporteInventarioViewModel.Ruta, typeof(ReporteInventarioPage));

            _sesionService = sesionService;
            _sesionService.SesionExpirada += OnSesionExpirada;

            Loaded += OnLoaded;
        }

        // Al abrir la app: si hay una sesión guardada y vigente, ir directo al panel
        private async void OnLoaded(object? sender, EventArgs e)
        {
            if (await _sesionService.RestaurarSesionAsync())
                await GoToAsync("//panel");
        }

        // La API rechazó el token (vencido o inválido): avisar y volver al login
        private void OnSesionExpirada(object? sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlertAsync("Sesión expirada",
                    "Tu sesión expiró. Volvé a iniciar sesión.", "Aceptar");
                await GoToAsync("//login");
            });
        }
    }
}