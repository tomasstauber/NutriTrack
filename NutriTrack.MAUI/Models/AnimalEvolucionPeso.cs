namespace NutriTrack.MAUI.Models
{
    // Fila del reporte de evolución de peso (en el back: AnimalEvolucionPesoItemDTO).
    // Primer y último pesaje del animal dentro del período
    public class AnimalEvolucionPeso
    {
        // Ya viene armada como "CUIG-NRO"
        public string Caravana { get; set; } = string.Empty;
        public DateTime FechaInicial { get; set; }
        public decimal PesoInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public decimal PesoFinal { get; set; }

        // R4: null si hay un solo registro en el período
        public decimal? VariacionKg { get; set; }
        public int CantidadRegistros { get; set; }

        // No viene de la API: variación con un decimal, igual que el PDF del back ("12,5", "-3,0") o "—" si no hay
        public string VariacionTexto => VariacionKg is { } variacion
            ? variacion.ToString("0.0")
            : "—";
    }
}
