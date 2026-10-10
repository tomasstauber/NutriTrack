using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class AlertaService : ApiServiceBase, IAlertaService
    {
        public AlertaService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<AlertasVigentes>> ListarAsync() =>
            GetAsync<AlertasVigentes>("api/Alerta");
    }
}
