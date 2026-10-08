namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/PlanAlimenticio (NutriTrack.API.DTOs.PlanAlimenticioDTO)
    public class PlanAlimenticioRequest
    {
        public string NombrePlan { get; set; } = string.Empty;

        // Opcionales: vacíos viajan en null
        public string? Categoria { get; set; }
        public decimal? PesoVivoInicialPromedio { get; set; }
        public decimal? PesoObjetivo { get; set; }
        public decimal? GananciaPesoEsperada { get; set; }

        public string TipoAlimentacion { get; set; } = string.Empty;

        public string? TiempoAlimentacion { get; set; }

        public decimal KgMsDiariaPorAnimal { get; set; }

        public string? Observaciones { get; set; }

        // Componentes del plan. En el back se llama "Detalle", en singular
        public List<PlanAlimenticioDetalleRequest> Detalle { get; set; } = [];
    }
}
