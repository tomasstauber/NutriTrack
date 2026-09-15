// Core.Entities/DetalleMedicamento.cs
using System;

namespace NutriTrack.Core.Entities
{
    public class DetalleMedicamento
    {
        public int Id { get; set; }
        public decimal? Dosis { get; set; }
        public string? Unidad { get; set; }
        public string? Observaciones { get; set; }

        public int IdEventoSanitario { get; set; }
        public int IdMedicamento { get; set; }
    }
}