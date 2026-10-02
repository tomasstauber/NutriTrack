using System.Text.Json;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class SesionService : ISesionService
    {
        private const string ClaveAlmacenamiento = "sesion_nutritrack";

        public SesionUsuario? SesionActual { get; private set; }

        public bool HaySesionActiva => SesionActual is { EstaVigente: true };

        public event EventHandler? SesionExpirada;

        public async Task IniciarSesionAsync(SesionUsuario sesion)
        {
            SesionActual = sesion;

            try
            {
                var json = JsonSerializer.Serialize(sesion);
                await SecureStorage.Default.SetAsync(ClaveAlmacenamiento, json);
            }
            catch (Exception)
            {
                // Si el almacenamiento seguro falla, la sesión sigue
                // funcionando en memoria; solo no se recordará al reabrir.
            }
        }

        public async Task<bool> RestaurarSesionAsync()
        {
            try
            {
                var json = await SecureStorage.Default.GetAsync(ClaveAlmacenamiento);
                if (string.IsNullOrEmpty(json))
                    return false;

                var sesion = JsonSerializer.Deserialize<SesionUsuario>(json);
                if (sesion is null || !sesion.EstaVigente)
                {
                    CerrarSesion();
                    return false;
                }

                SesionActual = sesion;
                return true;
            }
            catch (Exception)
            {
                CerrarSesion();
                return false;
            }
        }

        public void CerrarSesion()
        {
            SesionActual = null;
            SecureStorage.Default.Remove(ClaveAlmacenamiento);
        }

        public void NotificarSesionExpirada()
        {
            CerrarSesion();
            SesionExpirada?.Invoke(this, EventArgs.Empty);
        }
    }
}