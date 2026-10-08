namespace NutriTrack.MAUI.Models
{
    // Fila del reporte de inventario (en el back: AnimalInventarioItemDTO)
    public class AnimalInventario
    {
        // Ya viene armada como "CUIG-NRO"
        public string Caravana { get; set; } = string.Empty;
        public string Raza { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;

        // Texto ya armado por el back, por ejemplo "3 años y 2 meses"
        public string Edad { get; set; } = string.Empty;
        public DateTime FechaAltaSistema { get; set; }

        // "Activo" o "Inactivo"
        public string EstadoActual { get; set; } = string.Empty;

        // null si el animal no tiene rodeo
        public string? RodeoActual { get; set; }

        // No viene de la API: texto de la columna rodeo
        public string RodeoTexto => string.IsNullOrEmpty(RodeoActual) ? "Sin rodeo" : RodeoActual;
    }
}
