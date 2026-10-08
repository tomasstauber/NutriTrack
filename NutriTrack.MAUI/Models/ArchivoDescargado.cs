namespace NutriTrack.MAUI.Models
{
    // Archivo que devuelve la API (por ejemplo, el PDF de un reporte).
    // Lo arma ApiServiceBase.GetArchivoAsync a partir de los bytes y los encabezados de la respuesta
    public class ArchivoDescargado
    {
        public byte[] Contenido { get; init; } = [];

        // Nombre del encabezado Content-Disposition (null si el back no lo manda)
        public string? NombreSugerido { get; init; }

        // Content-Type de la respuesta, por ejemplo "application/pdf"
        public string TipoContenido { get; init; } = string.Empty;
    }
}
