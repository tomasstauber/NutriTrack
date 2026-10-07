namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Animal (en el back: CrearAnimalDTO).
    // La fecha de alta no se envía: la pone el sistema (R5)
    public class CrearAnimalRequest
    {
        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;

        // DateOnly viaja en ISO solo fecha ("2026-10-05")
        public DateOnly FechaNacimiento { get; set; }
        public decimal PesoAlNacer { get; set; }

        // Madre y padre: las dos partes o ninguna; sin informar viajan en null
        public string? CaravanaCuigMadre { get; set; }
        public string? CaravanaNroManejoMadre { get; set; }
        public string? CaravanaCuigPadre { get; set; }
        public string? CaravanaNroManejoPadre { get; set; }

        public string Raza { get; set; } = string.Empty;

        // Texto "Macho" / "Hembra": el openapi.json dice integer, pero el back lo lee como texto
        public string Sexo { get; set; } = string.Empty;

        // Opcional: vacío viaja en null
        public string? ColorPelaje { get; set; }
    }
}
