using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class IngredienteService : ApiServiceBase, IIngredienteService
    {
        public IngredienteService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<List<Ingrediente>>> ListarAsync() =>
            GetAsync<List<Ingrediente>>("api/Ingrediente");

        public Task<ResultadoApi<Ingrediente>> CrearAsync(IngredienteRequest ingrediente) =>
            PostAsync<Ingrediente>("api/Ingrediente", ingrediente);

        public Task<ResultadoApi<bool>> ActualizarAsync(int id, IngredienteRequest ingrediente) =>
            PutSinRespuestaAsync($"api/Ingrediente/{id}", ingrediente);

        public Task<ResultadoApi<string>> DesactivarAsync(int id) =>
            DeleteAsync<string>($"api/Ingrediente/{id}");
    }
}
