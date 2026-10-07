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
            builder.Services.AddTransient<IRodeoService, RodeoService>();

            // ViewModels
            builder.Services.AddTransient<IngredientesViewModel>();
            builder.Services.AddTransient<RodeosViewModel>();
            // Servicios que hablan con la API
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IAnimalService, AnimalService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<PanelPrincipalViewModel>();
            builder.Services.AddTransient<AnimalesViewModel>();
            // Selector múltiple de animales: cada pantalla anfitriona recibe el suyo
            builder.Services.AddTransient<SelectorAnimalesViewModel>();

            // Navegación y pantallas
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<PanelPrincipalPage>();
            builder.Services.AddTransient<IngredientesPage>();
            builder.Services.AddTransient<AnimalesPage>();
            builder.Services.AddTransient<RodeosPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}