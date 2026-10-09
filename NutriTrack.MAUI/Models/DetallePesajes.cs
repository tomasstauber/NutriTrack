namespace NutriTrack.MAUI.Models
{
    // Detalle de pesajes del reporte de evolución de peso (en el back: DetallePesajesDTO).
    // Solo llega cuando se pide el reporte con Caravana
    public class DetallePesajes
    {
        // Ordenados por fecha ascendente
        public List<PesajeDetalle> Pesajes { get; set; } = [];
    }
}
