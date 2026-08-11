using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NutriTrack.Infraestructure.Services
{
    public class AlertaSanitariaWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AlertaSanitariaWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    var alertaService =
                        scope.ServiceProvider
                            .GetRequiredService<AlertaSanitariaService>();

                    var alertas = await alertaService.VerificarAlertasAsync(7);

                    foreach (var alerta in alertas)
                    {
                        Console.WriteLine($"[ALERTA SANITARIA] {alerta}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Error al verificar alertas sanitarias: {ex.Message}");
                }

                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }
    }
}