namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de GET api/ReporteFechasImportantes (en el back: ReporteFechasImportantesResponseDTO).
    // Si las dos listas quedan vacías el back responde 404; si solo una, llega vacía en el 200.
    // Las listas no vienen ordenadas
    public class ReporteFechasImportantes
    {
        // S1
        public List<ProximaAplicacion> ProximasAplicaciones { get; set; } = [];

        // S2
        public List<VencimientoEvento> VencimientosEventos { get; set; } = [];
    }
}
