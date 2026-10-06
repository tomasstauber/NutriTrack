using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class RodeoService : ApiServiceBase, IRodeoService
    {
        public RodeoService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<List<Rodeo>>> ListarAsync() =>
            GetAsync<List<Rodeo>>("api/Rodeo");

        public Task<ResultadoApi<ResumenEliminacionRodeo>> ObtenerResumenEliminacionAsync(int idRodeo) =>
            GetAsync<ResumenEliminacionRodeo>($"api/EliminarRodeo/{idRodeo}");

        public Task<ResultadoApi<bool>> EliminarAsync(int idRodeo) =>
            DeleteAsync($"api/EliminarRodeo/{idRodeo}", new EliminarRodeoRequest { Confirmar = true });
    }
}
