using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NutriTrack.Core.Entities
{
    public class PlanAlimenticioDetalle
    {
        public int Id { get; set; }
        public decimal PorcentajeInclusionMs { get; set; }
        public string? Observaciones { get; set; }
        public int IdPlanAlimenticio { get; set; }
        public int IdIngrediente { get; set; }
        public PlanAlimenticio? PlanAlimenticio { get; set; }
    }
}
