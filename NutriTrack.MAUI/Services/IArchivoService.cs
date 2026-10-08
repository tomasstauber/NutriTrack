using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Guarda un archivo descargado de la API y lo abre con la aplicación del sistema
    // (en Windows, el visor de PDF por defecto). Sin paquetes extra: FileSystem + Launcher de MAUI
    public interface IArchivoService
    {
        // Guarda en la caché de la app y lo abre. Devuelve la ruta donde quedó guardado.
        // ResultadoApi en error si no se pudo guardar o si no hay una aplicación para abrirlo
        Task<ResultadoApi<string>> GuardarYAbrirAsync(ArchivoDescargado archivo, string nombrePorDefecto);
    }
}
