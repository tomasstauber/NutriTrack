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
            return ListarAsync(texto: null, pagina: pagina, tamanioPagina: tamanioPagina,
                incluirInactivos: incluirInactivos);
        }

        public Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            string? texto, int? idRodeo = null, bool sinRodeo = false,
            bool incluirInactivos = false, int pagina = 1, int tamanioPagina = 50)
        {
            var url = $"api/Animal?pagina={pagina}&tamanioPagina={tamanioPagina}";

            // El back busca sobre cuig y número pegados, sin guion:
            // "AR001-00001" (como se muestra) tiene que viajar como "AR00100001"
            var textoBusqueda = texto?.Replace("-", string.Empty).Trim();
            if (!string.IsNullOrEmpty(textoBusqueda))
                url += $"&texto={Uri.EscapeDataString(textoBusqueda)}";

            if (idRodeo.HasValue)
                url += $"&idRodeo={idRodeo.Value}";

            if (sinRodeo)
                url += "&sinRodeo=true";

            if (incluirInactivos)
                url += "&incluirInactivos=true";

            return GetAsync<ListadoPaginado<AnimalListado>>(url);
        }

        public Task<ResultadoApi<FichaAnimal>> ObtenerFichaAsync(string cuig, string nroManejo)
        {
            var url = $"api/ConsultaFichaIndividualAnimal?cuig={Uri.EscapeDataString(cuig)}" +
                      $"&nroManejo={Uri.EscapeDataString(nroManejo)}";

            return GetAsync<FichaAnimal>(url);
        }

        public async Task<ResultadoApi<AnimalListado?>> BuscarActivoPorCaravanaAsync(string cuig, string nroManejo)
        {
            // texto busca en forma parcial (ILIKE): puede traer otras caravanas parecidas,
            // así que se queda con la que coincide completa, sin distinguir mayúsculas
            // ("ar001" encuentra "AR001", como la búsqueda del back)
            var resultado = await ListarAsync(texto: cuig + nroManejo);

            if (!resultado.Exito)
                return ResultadoApi<AnimalListado?>.Error(
                    resultado.MensajeError ?? "Ocurrió un error inesperado.", resultado.CodigoEstado);

            var animal = resultado.Datos?.Items.FirstOrDefault(a =>
                string.Equals(a.CaravanaCuig, cuig, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(a.CaravanaNroManejo, nroManejo, StringComparison.OrdinalIgnoreCase));

            return ResultadoApi<AnimalListado?>.Ok(animal);
        }
    }
}
