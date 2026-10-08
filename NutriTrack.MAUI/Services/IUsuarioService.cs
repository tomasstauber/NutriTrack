using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IUsuarioService
    {
        // Lista los usuarios activos. Solo para Administrador (si no, 403).
        Task<ResultadoApi<List<Usuario>>> ListarAsync();

        // Crea un usuario. Solo para Administrador. Devuelve el usuario creado.
        Task<ResultadoApi<Usuario>> CrearAsync(CrearUsuarioRequest pedido);

        // Elimina un usuario. Solo para Administrador. La confirmación va en la query.
        Task<ResultadoApi<bool>> EliminarAsync(int id);
    }
}
