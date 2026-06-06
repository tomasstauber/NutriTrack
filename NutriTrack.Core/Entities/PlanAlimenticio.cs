using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class PlanAlimenticio
    {
        public int Id { get; set; }
        public string NombrePlan { get; set; }
        public string? Categoria { get; set; }
        public decimal? PesoVivoInicialPromedio { get; set; }
        public decimal? PesoObjetivo { get; set; }
        public decimal? GananciaPesoEsperada { get; set; }
        public string TipoAlimentacion { get; set; }
        public string? TiempoAlimentacion { get; set; }
        public decimal KgMsDiariaPorAnimal { get; set; }
        public string? Observaciones { get; set; }
        public List<PlanAlimenticioDetalle> Detalles { get; set; } = new();

    }
}
