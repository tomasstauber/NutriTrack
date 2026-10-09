namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de GET api/ReporteEvolucionPeso (en el back: ReporteEvolucionPesoResponseDTO).
    // Sin registros en el período el back responde 404, no un 200 vacío
    public class ReporteEvolucionPeso
    {
        // S1, ya ordenados por caravana
        public List<AnimalEvolucionPeso> Animales { get; set; } = [];

        // S2: null salvo cuando se pide con Caravana
        public DetallePesajes? Detalle { get; set; }
    }
}
