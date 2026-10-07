using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class UsuarioService : ApiServiceBase, IUsuarioService
    {
        public UsuarioService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<List<Usuario>>> ListarAsync() =>
            GetAsync<List<Usuario>>("api/Usuario");
    }
}
