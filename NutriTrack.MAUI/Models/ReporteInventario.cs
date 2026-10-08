namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de GET api/ReporteInventarioAnimales (en el back: ReporteInventarioAnimalesResponseDTO).
    // Sin resultados el back responde 404, no un 200 vacío
    public class ReporteInventario
    {
        public int TotalAnimales { get; set; }
        public List<AnimalInventario> Animales { get; set; } = [];
    }
}
