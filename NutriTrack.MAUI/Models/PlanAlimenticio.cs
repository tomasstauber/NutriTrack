namespace NutriTrack.MAUI.Models
{
    // Plan alimenticio de la lista, tal como lo devuelve GET api/PlanAlimenticio
    // (NutriTrack.API.DTOs.PlanAlimenticioListaResponseDTO).
    public class PlanAlimenticio
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string TipoAlimentacion { get; set; } = string.Empty;
        public decimal KgMsDiariaPorAnimal { get; set; }
    }
}
