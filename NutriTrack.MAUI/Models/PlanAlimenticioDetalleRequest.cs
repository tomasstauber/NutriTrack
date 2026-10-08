namespace NutriTrack.MAUI.Models
{
    // Componente del plan dentro de POST api/PlanAlimenticio
    // (NutriTrack.API.DTOs.PlanAlimenticioDetalleDTO)
    public class PlanAlimenticioDetalleRequest
    {
        public int IdIngrediente { get; set; }
        public decimal PorcentajeInclusionMs { get; set; }
        public string? Observaciones { get; set; }
    }
}
