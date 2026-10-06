using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IAuthService
    {
        // Valida las credenciales contra la API y, si son correctas,
        // guarda la sesión. Devuelve la sesión iniciada o un mensaje de error.
        Task<ResultadoApi<SesionUsuario>> IniciarSesionAsync(string nombreUsuario, string contrasenia);
    }
}
