using Microsoft.Extensions.Logging;
using NutriTrack.MAUI.Services;

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

            // Cliente HTTP para hablar con NutriTrack.API.
            // Se configura una sola vez acá y todos los servicios lo reutilizan.
            builder.Services.AddHttpClient("NutriTrackApi", client =>
            {
                client.BaseAddress = new Uri(ApiConfig.BaseUrl);
                client.Timeout = TimeSpan.FromSeconds(15);
            });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}