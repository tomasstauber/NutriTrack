using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class ArchivoService : IArchivoService
    {
        public async Task<ResultadoApi<string>> GuardarYAbrirAsync(ArchivoDescargado archivo, string nombrePorDefecto)
        {
            // Path.GetFileName: el nombre viene de un encabezado, así no puede apuntar fuera de la caché
            var nombre = Path.GetFileName(archivo.NombreSugerido ?? string.Empty);
            if (string.IsNullOrWhiteSpace(nombre))
                nombre = nombrePorDefecto;

            var ruta = Path.Combine(FileSystem.CacheDirectory, nombre);

            try
            {
                await File.WriteAllBytesAsync(ruta, archivo.Contenido);
            }
            catch (IOException)
            {
                // Por ejemplo, el mismo PDF todavía abierto en el visor
                return ResultadoApi<string>.Error(
                    "No se pudo guardar el archivo. Si está abierto, cerralo e intentá de nuevo.");
            }

            // La caché la puede limpiar el sistema: desde el visor se puede "Guardar como"
            bool abierto;
            try
            {
                abierto = await Launcher.Default.OpenAsync(
                    new OpenFileRequest(nombre, new ReadOnlyFile(ruta, archivo.TipoContenido)));
            }
            catch (Exception)
            {
                // Una falla del sistema al abrir no tiene que cerrar la app: el archivo ya está guardado
                abierto = false;
            }

            return abierto
                ? ResultadoApi<string>.Ok(ruta)
                : ResultadoApi<string>.Error(
                    $"El archivo se guardó, pero no hay una aplicación para abrirlo.\n\n{ruta}");
        }
    }
}
