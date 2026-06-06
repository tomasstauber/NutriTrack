namespace NutriTrack.API.DTOs
{
    public class PlanAlimenticioDetalleResponseDTO
    {
        public int Id { get; set; }
        public int IdIngrediente { get; set; }

        public decimal PorcentajeInclusionMs { get; set; }
        public string? Observaciones { get; set; }
    }
}
