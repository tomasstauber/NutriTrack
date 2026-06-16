namespace NutriTrack.API.DTOs
{
    public class CrearAnimalDTO
    {
        public string CaravanaCuig { get; set; }
        public string CaravanaNroManejo { get; set;}
        public DateTime FechaNacimiento { get; set;}
        public float PesoAlNacer {  get; set;}
        public string? CaravanaCuigMadre { get; set;}
        public string? CaravanaNroManejoMadre { get; set;}
        public string? CaravanaCuigPadre { get; set;}
        public string? CaravanaNroManejoPadre { get; set;}
        public string Raza {  get; set;}
        public string Sexo { get; set;}
        public string? ColorPelaje { get; set;}
    }
}
