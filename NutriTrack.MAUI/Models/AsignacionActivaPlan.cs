namespace NutriTrack.MAUI.Models
{
    // Asignación activa de un plan, tal como la devuelve
    // GET api/PlanRodeoAsignacion/plan/{id}/activas (en el back: AsignacionActivaPlanResponseDTO)
    public class AsignacionActivaPlan
    {
        public int IdAsignacion { get; set; }
        public int IdRodeo { get; set; }
        public string NombreRodeo { get; set; } = string.Empty;
        public string? DescripcionRodeo { get; set; }

        // Solo animales activos, como en GET api/Rodeo
        public int CantidadAnimales { get; set; }

        public DateOnly VigenciaDesde { get; set; }

        // null si la asignación no tiene fin
        public DateOnly? VigenciaHasta { get; set; }

        // Vigencia para mostrar. No viene de la API: se arma acá
        public string TextoVigenciaDesde => VigenciaDesde.ToString("dd/MM/yyyy");
        public string TextoVigenciaHasta => VigenciaHasta?.ToString("dd/MM/yyyy") ?? "—";
    }
}
