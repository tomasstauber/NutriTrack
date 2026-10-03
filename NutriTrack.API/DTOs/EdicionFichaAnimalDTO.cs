using NutriTrack.Core.Entities.Enums;
using System.Data;

namespace NutriTrack.API.DTOs
{
    public class EdicionFichaAnimalDTO
    {
        public DateTime FechaNacimiento { get; set; }
        public decimal PesoAlNacer { get; set; }
        public Sexo Sexo {  get; set; }
        public string Raza { get; set; }
        public string? ColorPelaje { get; set; }
        public string? CaravanaCuigMadre { get; set; }
        public string? CaravanaNroManejoMadre { get; set; }
        public string? CaravanaCuigPadre { get; set; }
        public string? CaravanaNroManejoPadre { get; set; }
    }
}
