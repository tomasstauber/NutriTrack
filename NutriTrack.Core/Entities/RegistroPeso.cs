using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class RegistroPeso
    {
        public int Id { get; set; }
        public DateTime FechaPesaje { get; set; }
        //se hizo un cambio de tipo de dato paso de decimal a float. Ya que double precision equivale a float en c#.
        public float PesoKg { get; set; }
        public string? Observaciones { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }
    }
}
