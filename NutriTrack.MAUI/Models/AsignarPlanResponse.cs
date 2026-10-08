namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de POST api/PlanRodeoAsignacion (en el back: AsignarPlanResponseDTO).
    // Cantidad de animales y kg de MS total los calcula el back (R5)
    public class AsignarPlanResponse
    {
        // null si el rodeo no tenía otro plan activo (R7 / S8)
        public string? PlanAnteriorReemplazado { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public string NombreRodeo { get; set; } = string.Empty;
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
        public int CantidadAnimales { get; set; }
        public decimal KgMsDiariaTotal { get; set; }
    }
}
