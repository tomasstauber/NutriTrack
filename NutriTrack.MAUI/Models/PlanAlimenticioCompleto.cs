namespace NutriTrack.MAUI.Models
{
    // Plan con sus componentes, tal como lo devuelve GET api/PlanAlimenticio/{id}
    // (NutriTrack.API.DTOs.PlanAlimenticioResponseDTO). Es solo de lectura:
    // para el PUT se arma un PlanAlimenticioRequest
    public class PlanAlimenticioCompleto
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public decimal? PesoVivoInicialPromedio { get; set; }
        public decimal? PesoObjetivo { get; set; }
        public decimal? GananciaPesoEsperada { get; set; }
        public string TipoAlimentacion { get; set; } = string.Empty;
        public string? TiempoAlimentacion { get; set; }
        public decimal KgMsDiariaPorAnimal { get; set; }
        public string? Observaciones { get; set; }

        // Rodeos que tienen este plan asignado y vigente (R8)
        public int AsignacionesVigentes { get; set; }

        // Componentes del plan. Al leer se llama "Detalles", en plural
        // (al enviar es "Detalle", en singular)
        public List<PlanAlimenticioDetalle> Detalles { get; set; } = [];
    }
}
