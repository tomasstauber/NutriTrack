using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    // Entidad simplificada para relacionarse con Rodeo
    // Cuando haga el CRUD de animal van a estar todos los campos
    public class Animal
    {
        public int Id { get; set; }
        public string CaravanaCuig { get; set; }
        public string CaravanaNroManejo { get; set; }
        public bool Estado { get; set; }
        public int? RodeoId { get; set; }
        public Rodeo? Rodeo { get; set; }
    }
}