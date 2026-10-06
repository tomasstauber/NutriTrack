namespace NutriTrack.MAUI.Models
{
    // Fila del listado de GET api/Animal (en el back: AnimalListadoDTO)
    public class AnimalListado
    {
        public int Id { get; set; }
        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;
        public string? Raza { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public bool Estado { get; set; }
        public int? RodeoId { get; set; }
        public string? RodeoNombre { get; set; }
    }
}
