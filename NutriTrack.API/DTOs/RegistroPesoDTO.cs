namespace NutriTrack.API.DTOs
{
    public class RegistroPesoDTO
    {
        public DateTime FechaPesaje { get; set; }
        public float PesoKg { get; set; }
        public string? Observaciones { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }
    }
}