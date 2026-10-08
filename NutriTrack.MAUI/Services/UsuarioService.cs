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

        public Task<ResultadoApi<Usuario>> CrearAsync(CrearUsuarioRequest pedido) =>
            PostAsync<Usuario>("api/Usuario", pedido);

        public Task<ResultadoApi<Usuario>> EditarAsync(int id, EditarUsuarioRequest pedido) =>
            PutAsync<Usuario>($"api/Usuario/{id}", pedido);
        public Task<ResultadoApi<bool>> EliminarAsync(int id) =>
            DeleteAsync($"api/Usuario/{id}?confirmar=true");
    }
}
