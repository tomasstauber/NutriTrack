using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class RegistroPesoService : ApiServiceBase, IRegistroPesoService
    {
        public RegistroPesoService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        // El mensaje de éxito se arma con lo enviado, así que no se lee la respuesta
        public Task<ResultadoApi<bool>> RegistrarAsync(RegistroPesoRequest registro) =>
            PostSinRespuestaAsync("api/RegistroPeso", registro);
    }
}
