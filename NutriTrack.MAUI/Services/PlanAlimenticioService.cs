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

        public Task<ResultadoApi<bool>> CrearAsync(PlanAlimenticioRequest plan) =>
            PostSinRespuestaAsync("api/PlanAlimenticio", plan);

        public Task<ResultadoApi<PlanAlimenticioCompleto>> ObtenerAsync(int id) =>
            GetAsync<PlanAlimenticioCompleto>($"api/PlanAlimenticio/{id}");

        public Task<ResultadoApi<bool>> ActualizarAsync(int id, PlanAlimenticioRequest plan) =>
            PutSinRespuestaAsync($"api/PlanAlimenticio/{id}", plan);

        public Task<ResultadoApi<AsignarPlanResponse>> AsignarARodeoAsync(AsignarPlanRequest asignacion) =>
            PostAsync<AsignarPlanResponse>("api/PlanRodeoAsignacion", asignacion);
    }
}
