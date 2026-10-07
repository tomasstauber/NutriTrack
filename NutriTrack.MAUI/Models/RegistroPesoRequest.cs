namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/RegistroPeso (en el back: RegistroPesoDTO).
    // El usuario que registra lo toma el back del token: no se envía
    public class RegistroPesoRequest
    {
        // DateOnly viaja en ISO solo fecha ("2026-10-06")
        public DateOnly FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }

        // Opcional: vacías viajan en null
        public string? Observaciones { get; set; }

        public int IdAnimal { get; set; }
    }
}
