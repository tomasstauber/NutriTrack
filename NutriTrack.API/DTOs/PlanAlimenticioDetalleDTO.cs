namespace NutriTrack.API.DTOs
{
    public class PlanAlimenticioDetalleDTO
    {
        public int IdIngrediente { get; set; }
        public decimal PorcentajeInclusionMs { get; set; }
        public string? Observaciones { get; set; }
    }
}