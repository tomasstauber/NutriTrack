namespace NutriTrack.API.DTOs
{
    public class AsignarPlanResponseDTO
    {
        public string? PlanAnteriorReemplazado { get; set; }
        public string NombrePlan { get; set; }
        public string NombreRodeo { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
        public int CantidadAnimales { get; set; }
        public decimal KgMsDiariaTotal { get; set; }
    }
}
