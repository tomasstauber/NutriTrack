using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class RegistroPeso
    {
        public int Id { get; set; }
        public DateTime FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }
        public string? Observaciones { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }
    }
}
