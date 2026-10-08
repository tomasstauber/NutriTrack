namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/PlanRodeoAsignacion (en el back: AsignarPlanDTO)
    public class AsignarPlanRequest
    {
        public int IdPlanAlimenticio { get; set; }
        public int IdRodeo { get; set; }

        // DateOnly viaja en ISO solo fecha ("2026-07-01")
        public DateOnly VigenciaDesde { get; set; }

        // Opcional: sin cargar viaja en null
        public DateOnly? VigenciaHasta { get; set; }
    }
}
