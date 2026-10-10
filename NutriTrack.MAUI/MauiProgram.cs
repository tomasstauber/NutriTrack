using Microsoft.Extensions.Logging;
using NutriTrack.MAUI.Services;
using NutriTrack.MAUI.ViewModels;
using NutriTrack.MAUI.Views;

namespace NutriTrack.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Sesión del usuario: Singleton porque tiene que ser UNA sola
            // instancia compartida por toda la app
            builder.Services.AddSingleton<ISesionService, SesionService>();

            // Handler que agrega el token a cada pedido
            builder.Services.AddTransient<AuthHeaderHandler>();

            // Cliente HTTP para hablar con NutriTrack.API
            // Se configura una sola vez acá y todos los servicios lo reutilizan
            builder.Services.AddHttpClient("NutriTrackApi", client =>
            {
                client.BaseAddress = new Uri(ApiConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .AddHttpMessageHandler<AuthHeaderHandler>();

            // Servicios de la API
            builder.Services.AddTransient<IIngredienteService, IngredienteService>();
            builder.Services.AddTransient<IMedicamentoService, MedicamentoService>();

            // ViewModels
            builder.Services.AddTransient<IngredientesViewModel>();
            builder.Services.AddTransient<NuevoIngredienteViewModel>();
            builder.Services.AddTransient<MedicamentosViewModel>();
            builder.Services.AddTransient<NuevoMedicamentoViewModel>();
            builder.Services.AddTransient<IRodeoService, RodeoService>();
            builder.Services.AddTransient<IUsuarioService, UsuarioService>();

            // ViewModels
            builder.Services.AddTransient<IngredientesViewModel>();
            builder.Services.AddTransient<RodeosViewModel>();
            builder.Services.AddTransient<CrearRodeoViewModel>();
            builder.Services.AddTransient<TransferirAnimalesViewModel>();
            builder.Services.AddTransient<UsuariosViewModel>();
            builder.Services.AddTransient<NuevoUsuarioViewModel>();
            builder.Services.AddTransient<EditarUsuarioViewModel>();
            // Servicios que hablan con la API
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IAnimalService, AnimalService>();
            builder.Services.AddTransient<IRegistroPesoService, RegistroPesoService>();
            builder.Services.AddTransient<IEventoSanitarioService, EventoSanitarioService>();
            builder.Services.AddTransient<IPlanAlimenticioService, PlanAlimenticioService>();
            builder.Services.AddTransient<IReporteService, ReporteService>();
            builder.Services.AddTransient<IArchivoService, ArchivoService>();
            builder.Services.AddTransient<IAlertaService, AlertaService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<PanelPrincipalViewModel>();
            builder.Services.AddTransient<AnimalesViewModel>();
            builder.Services.AddTransient<AgregarAnimalViewModel>();
            builder.Services.AddTransient<PerfilAnimalViewModel>();
            builder.Services.AddTransient<EditarAnimalViewModel>();
            builder.Services.AddTransient<RegistrarPesoViewModel>();
            builder.Services.AddTransient<RegistrarEventoSanitarioViewModel>();
            builder.Services.AddTransient<PlanesAlimenticiosViewModel>();
            builder.Services.AddTransient<ReportesViewModel>();
            builder.Services.AddTransient<ReporteInventarioViewModel>();
            builder.Services.AddTransient<ReporteFechasImportantesViewModel>();
            builder.Services.AddTransient<ReporteEvolucionPesoViewModel>();
            // Filtro de período de los reportes: cada reporte recibe el suyo
            builder.Services.AddTransient<FiltroPeriodoReporteViewModel>();
            builder.Services.AddTransient<AsignarPlanRodeoViewModel>();
            builder.Services.AddTransient<NuevoPlanAlimenticioViewModel>();
            builder.Services.AddTransient<FichaPlanAlimenticioViewModel>();
            builder.Services.AddTransient<RodeoAsignadoViewModel>();
            builder.Services.AddTransient<AlertasViewModel>();
            // Selector múltiple de animales: cada pantalla anfitriona recibe el suyo
            builder.Services.AddTransient<SelectorAnimalesViewModel>();

            // Navegación y pantallas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PanelPrincipalPage>();
            builder.Services.AddTransient<IngredientesPage>();
            builder.Services.AddTransient<NuevoIngredientePage>();
            builder.Services.AddTransient<MedicamentosPage>();
            builder.Services.AddTransient<NuevoMedicamentoPage>();
            builder.Services.AddTransient<AnimalesPage>();
            builder.Services.AddTransient<AgregarAnimalPage>();
            builder.Services.AddTransient<PerfilAnimalPage>();
            builder.Services.AddTransient<EditarAnimalPage>();
            builder.Services.AddTransient<RodeosPage>();
            builder.Services.AddTransient<CrearRodeoPage>();
            builder.Services.AddTransient<TransferirAnimalesPage>();
            builder.Services.AddTransient<UsuariosPage>();
            builder.Services.AddTransient<NuevoUsuarioPage>();
            builder.Services.AddTransient<EditarUsuarioPage>();
            builder.Services.AddTransient<RegistrarPesoPage>();
            builder.Services.AddTransient<RegistrarEventoSanitarioPage>();
            builder.Services.AddTransient<PlanesAlimenticiosPage>();
            builder.Services.AddTransient<ReportesPage>();
            builder.Services.AddTransient<ReporteInventarioPage>();
            builder.Services.AddTransient<ReporteFechasImportantesPage>();
            builder.Services.AddTransient<ReporteEvolucionPesoPage>();
            builder.Services.AddTransient<AsignarPlanRodeoPage>();
            builder.Services.AddTransient<NuevoPlanAlimenticioPage>();
            builder.Services.AddTransient<FichaPlanAlimenticioPage>();
            builder.Services.AddTransient<RodeoAsignadoPage>();
            builder.Services.AddTransient<AlertasPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}