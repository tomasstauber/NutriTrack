namespace NutriTrack.API.DTOs
{
    public class RegistroPesoResponseDTO
    {
        public int Id { get; set; }
        public DateTime FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }
        public string? Observaciones { get; set; }
    }
}