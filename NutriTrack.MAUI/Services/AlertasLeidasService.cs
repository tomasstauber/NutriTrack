using System.Globalization;
using System.Text.Json;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Preferences y no SecureStorage: no son datos sensibles, y Preferences es sincrónico
    // y pensado para datos chicos de la app. En Windows la app no está empaquetada,
    // así que Preferences va a un archivo local, sin el límite de tamaño por valor de las empaquetadas
    public class AlertasLeidasService : IAlertasLeidasService
    {
        private const string PrefijoClave = "alertas_leidas_";

        private readonly ISesionService _sesionService;

        public AlertasLeidasService(ISesionService sesionService)
        {
            _sesionService = sesionService;
        }

        // Arreglo JSON y no un separador: un nombre de rodeo puede tener cualquier carácter,
        // y JSON escapa las comillas, así que dos alertas distintas nunca dan la misma clave.
        // Fecha en ISO e invariante, para no depender del idioma del equipo
        public string Clave(Alerta alerta) => JsonSerializer.Serialize(new[]
        {
            alerta.Tipo,
            alerta.Subtipo,
            alerta.Destino,
            alerta.Fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
        });

        public IReadOnlySet<string> ObtenerLeidas(IEnumerable<Alerta> vigentes)
        {
            var claveUsuario = ClaveUsuario();
            if (claveUsuario is null)
                return new HashSet<string>();

            var guardadas = Leer(claveUsuario);

            // Limpieza: solo quedan las que siguen llegando del back
            var clavesVigentes = vigentes.Select(Clave).ToHashSet();
            var leidas = guardadas.Where(clavesVigentes.Contains).ToHashSet();

            if (leidas.Count != guardadas.Count)
                Escribir(claveUsuario, leidas);

            return leidas;
        }

        public void GuardarLeidas(IEnumerable<string> claves)
        {
            var claveUsuario = ClaveUsuario();
            if (claveUsuario is not null)
                Escribir(claveUsuario, claves.ToHashSet());
        }

        // Por usuario. En minúsculas: el back compara el nombre de usuario sin distinguir mayúsculas
        private string? ClaveUsuario()
        {
            var nombreUsuario = _sesionService.SesionActual?.NombreUsuario;
            return string.IsNullOrWhiteSpace(nombreUsuario)
                ? null
                : PrefijoClave + nombreUsuario.Trim().ToLowerInvariant();
        }

        private static HashSet<string> Leer(string claveUsuario)
        {
            try
            {
                var json = Preferences.Default.Get<string?>(claveUsuario, null);
                if (string.IsNullOrEmpty(json))
                    return [];

                return JsonSerializer.Deserialize<HashSet<string>>(json) ?? [];
            }
            catch (JsonException)
            {
                // Dato corrupto: se descarta para que no vuelva a fallar; todo queda no leído
                Borrar(claveUsuario);
                return [];
            }
            catch (Exception)
            {
                // Falla de la plataforma: todo queda no leído
                return [];
            }
        }

        private static void Escribir(string claveUsuario, HashSet<string> claves)
        {
            try
            {
                if (claves.Count == 0)
                    Preferences.Default.Remove(claveUsuario);
                else
                    Preferences.Default.Set(claveUsuario, JsonSerializer.Serialize(claves));
            }
            catch (Exception)
            {
                // Si no se puede guardar, la pantalla sigue como se marcó; solo no se recuerda
            }
        }

        private static void Borrar(string claveUsuario)
        {
            try
            {
                Preferences.Default.Remove(claveUsuario);
            }
            catch (Exception)
            {
            }
        }
    }
}
