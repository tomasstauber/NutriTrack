namespace NutriTrack.MAUI.Models
{
    // Ficha de GET api/ConsultaFichaIndividualAnimal (en el back: ConsultaFichaIndividualAnimalDTO)
    // No trae el id del animal: se pide por caravana
    public class FichaAnimal
    {
        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public decimal PesoAlNacer { get; set; }
        public string Sexo { get; set; } = string.Empty;
        public string Raza { get; set; } = string.Empty;
        public DateTime FechaAlta { get; set; }
        public string? ColorPelaje { get; set; }

        // Texto "Activo" / "Inactivo" (en el listado, en cambio, es bool)
        public string Estado { get; set; } = string.Empty;

        public string? RodeoActual { get; set; }

        // Madre y padre llegan como texto "CUIG-NRO" (ej. AR001-00001), no como objeto; null si no tiene
        public string? Madre { get; set; }
        public string? Padre { get; set; }

        // null si el animal no tiene pesajes
        public UltimoPeso? UltimoPeso { get; set; }
    }
}
