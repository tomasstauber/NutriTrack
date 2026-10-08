using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class PlanAlimenticioService : ApiServiceBase, IPlanAlimenticioService
    {
        public PlanAlimenticioService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<List<PlanAlimenticio>>> ListarAsync() =>
            GetAsync<List<PlanAlimenticio>>("api/PlanAlimenticio");

        public Task<ResultadoApi<AsignarPlanResponse>> AsignarARodeoAsync(AsignarPlanRequest asignacion) =>
            PostAsync<AsignarPlanResponse>("api/PlanRodeoAsignacion", asignacion);
    }
}
