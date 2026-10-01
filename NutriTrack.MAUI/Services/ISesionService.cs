using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface ISesionService
    {
        // Datos del usuario logueado (null si no hay sesión).
        SesionUsuario? SesionActual { get; }

        bool HaySesionActiva { get; }

        // Se dispara cuando la API rechaza el token (vencido o inválido).
        event EventHandler? SesionExpirada;

        Task IniciarSesionAsync(SesionUsuario sesion);
        Task<bool> RestaurarSesionAsync();
        void CerrarSesion();
        void NotificarSesionExpirada();
    }
}