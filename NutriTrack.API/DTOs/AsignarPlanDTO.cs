namespace NutriTrack.API.DTOs
{
    public class AsignarPlanDTO
    {
        public int IdPlanAlimenticio { get; set; }
        public int IdRodeo { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
    }
}
