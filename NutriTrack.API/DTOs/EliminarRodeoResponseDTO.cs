namespace NutriTrack.API.DTOs
{
    public class EliminarRodeoResponseDTO
    {
        public required string NombreRodeo { get; set; }
        public int CantidadAnimales { get; set; }
        public int CantidadPlanes { get; set; }
    }
}