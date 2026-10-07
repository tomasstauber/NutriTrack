namespace NutriTrack.MAUI.Models
{
    // Cuerpo de PUT api/EdicionFichaAnimal (en el back: EdicionFichaAnimalDTO).
    // La caravana va en la URL y la fecha de alta no se edita (R6): ninguna de las dos viaja acá
    public class EditarAnimalRequest
    {
        // DateOnly viaja en ISO solo fecha ("2026-10-05")
        public DateOnly FechaNacimiento { get; set; }
        public decimal PesoAlNacer { get; set; }

        // Texto "Macho" / "Hembra"
        public string Sexo { get; set; } = string.Empty;

        public string Raza { get; set; } = string.Empty;

        // Opcional: vacío viaja en null
        public string? ColorPelaje { get; set; }

        // El PUT reemplaza madre y padre siempre: en null se borran.
        // Por eso los actuales se precargan y se reenvían aunque no se toquen
        public string? CaravanaCuigMadre { get; set; }
        public string? CaravanaNroManejoMadre { get; set; }
        public string? CaravanaCuigPadre { get; set; }
        public string? CaravanaNroManejoPadre { get; set; }
    }
}
