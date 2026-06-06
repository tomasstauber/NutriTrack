using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class PlanRodeoAsignacion
    {
        public int Id { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly VigenciaHasta { get; set; }
        public bool Activo { get; set; }

        public int IdPlanAlimenticio { get; set; }
        public int IdRodeo { get; set; }
        public PlanAlimenticio? PlanAlimenticio { get; set; }
        //public Rodeo? Rodeo { get; set; }
    }
}
