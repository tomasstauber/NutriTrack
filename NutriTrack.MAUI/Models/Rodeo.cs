namespace NutriTrack.MAUI.Models
{
    // Rodeo tal como lo devuelve GET api/Rodeo
    // (NutriTrack.API.DTOs.RodeoListadoResponseDTO). Solo llegan los activos.
    public class Rodeo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CantidadAnimales { get; set; }

        // No viene de la API: para mostrar la descripción solo si tiene
        public bool TieneDescripcion => !string.IsNullOrWhiteSpace(Descripcion);

        // No viene de la API: texto del Picker con la cantidad de animales,
        // para que un rodeo vacío se note antes de elegirlo
        public string TextoOpcion => CantidadAnimales switch
        {
            <= 0 => $"{Nombre} (vacío: sin animales)",
            1 => $"{Nombre} (1 animal)",
            _ => $"{Nombre} ({CantidadAnimales} animales)"
        };
    }
}
