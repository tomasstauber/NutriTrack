namespace NutriTrack.API.DTOs
{
    public class TransferenciaAnimalResponseDTO
    {
        public int Id { get; set; }
        public required string CaravanaCuig { get; set; }
        public required string CaravanaNroManejo { get; set; }
    }
}