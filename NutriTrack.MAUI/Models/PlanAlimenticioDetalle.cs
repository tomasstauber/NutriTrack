namespace NutriTrack.MAUI.Models
{
    // Componente del plan dentro de GET api/PlanAlimenticio/{id}
    // (NutriTrack.API.DTOs.PlanAlimenticioDetalleResponseDTO)
    public class PlanAlimenticioDetalle
    {
        public int Id { get; set; }
        public int IdIngrediente { get; set; }

        // Viene aunque el ingrediente esté inactivo
        public string? NombreIngrediente { get; set; }

        public decimal PorcentajeInclusionMs { get; set; }
        public string? Observaciones { get; set; }
    }
}
