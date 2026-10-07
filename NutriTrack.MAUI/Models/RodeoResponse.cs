namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de POST api/Rodeo (en el back: RodeoResponseDTO)
    public class RodeoResponse
    {
        public string Mensaje { get; set; } = string.Empty;
        public int Id { get; set; }
        public string NombreRodeo { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CantidadAnimales { get; set; }
    }
}
