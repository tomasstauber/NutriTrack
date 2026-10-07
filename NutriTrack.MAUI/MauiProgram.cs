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
            builder.Services.AddTransient<MedicamentosViewModel>();
            builder.Services.AddTransient<NuevoMedicamentoViewModel>();
            builder.Services.AddTransient<IRodeoService, RodeoService>();
            builder.Services.AddTransient<IUsuarioService, UsuarioService>();

            // ViewModels
            builder.Services.AddTransient<IngredientesViewModel>();
            builder.Services.AddTransient<RodeosViewModel>();
            builder.Services.AddTransient<UsuariosViewModel>();
            // Servicios que hablan con la API
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IAnimalService, AnimalService>();
            builder.Services.AddTransient<IRegistroPesoService, RegistroPesoService>();
            builder.Services.AddTransient<IEventoSanitarioService, EventoSanitarioService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<PanelPrincipalViewModel>();
            builder.Services.AddTransient<AnimalesViewModel>();
            builder.Services.AddTransient<AgregarAnimalViewModel>();
            builder.Services.AddTransient<PerfilAnimalViewModel>();
            builder.Services.AddTransient<RegistrarPesoViewModel>();
            // Selector múltiple de animales: cada pantalla anfitriona recibe el suyo
            builder.Services.AddTransient<SelectorAnimalesViewModel>();

            // Navegación y pantallas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PanelPrincipalPage>();
            builder.Services.AddTransient<IngredientesPage>();
            builder.Services.AddTransient<MedicamentosPage>();
            builder.Services.AddTransient<NuevoMedicamentoPage>();
            builder.Services.AddTransient<AnimalesPage>();
            builder.Services.AddTransient<AgregarAnimalPage>();
            builder.Services.AddTransient<PerfilAnimalPage>();
            builder.Services.AddTransient<RodeosPage>();
            builder.Services.AddTransient<UsuariosPage>();
            builder.Services.AddTransient<RegistrarPesoPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}