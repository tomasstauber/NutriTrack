using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class EventoSanitarioService : ApiServiceBase, IEventoSanitarioService
    {
        public EventoSanitarioService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<List<EventoHistorial>>> ObtenerHistorialAnimalAsync(int idAnimal) =>
            GetAsync<List<EventoHistorial>>($"api/EventoSanitario/animal/{idAnimal}");

        public Task<ResultadoApi<EventoSanitarioRegistrado>> RegistrarMultipleAsync(RegistrarEventoSanitarioRequest pedido) =>
            PostAsync<EventoSanitarioRegistrado>("api/EventoSanitario/multiple", pedido);
    }
}
