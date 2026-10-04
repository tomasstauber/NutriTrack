namespace NutriTrack.API.DTOs
{
    public class PlanAlimenticioListaResponseDTO
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public string TipoAlimentacion { get; set; } = string.Empty;
        public decimal KgMsDiariaPorAnimal { get; set; }
    }
}
