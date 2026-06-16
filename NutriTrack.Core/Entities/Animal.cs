using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Core.Entities
{
    public class Animal
    {
        public int Id { get; set; }
        public string CaravanaCuig { get; set; }
        public string CaravanaNroManejo { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public float PesoAlNacer { get; set; }
        public int? PadreId { get; set; }
        public Animal? Padre { get; set; }
        public int? MadreId { get; set; }
        public Animal? Madre { get; set; }
        public string Raza { get; set; }
        //se crea una entidad porque solo hay dos datos posibles("macho","hembra"
        public Sexo Sexo { get; set; }
        public string ColorPelaje { get; set; }
        public DateTime FechaAlta   { get; set; }
        public bool Estado { get; set; }
        public int? RodeoId { get; set; }
        public Rodeo? Rodeo { get; set; }
    }
}