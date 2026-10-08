namespace NutriTrack.MAUI.Models
{
    // Medicamento aplicado dentro de un pedido de evento sanitario (en el back: DetalleMedicamentoDTO)
    public class DetalleMedicamentoRequest
    {
        public int IdMedicamento { get; set; }

        // Opcionales. Sin dosis, dosis y unidad viajan en null (R10)
        public decimal? Dosis { get; set; }

        // Texto exacto del enum UnidadDosis del back ("ml", "UI"...)
        public string? Unidad { get; set; }

        public string? Observaciones { get; set; }
    }
}
