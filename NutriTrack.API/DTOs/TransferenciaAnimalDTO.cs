namespace NutriTrack.API.DTOs
{
    public class TransferenciaAnimalesDTO
    {
        public int IdRodeoOrigen { get; set; }
        public int IdRodeoDestino { get; set; }
        public required List<int> AnimalesIds { get; set; }
    }
}