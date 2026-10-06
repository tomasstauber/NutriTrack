using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class AnimalService : ApiServiceBase, IAnimalService
    {
        public AnimalService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            int pagina = 1, int tamanioPagina = 50, bool incluirInactivos = false)
        {
            var url = $"api/Animal?pagina={pagina}&tamanioPagina={tamanioPagina}";

            if (incluirInactivos)
                url += "&incluirInactivos=true";

            return GetAsync<ListadoPaginado<AnimalListado>>(url);
        }
    }
}
