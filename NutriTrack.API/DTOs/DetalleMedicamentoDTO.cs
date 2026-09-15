namespace NutriTrack.API.DTOs
{
    public class DetalleMedicamentoDTO
    {
        public int IdMedicamento { get; set; }
        public decimal? Dosis { get; set; }
        public string? Unidad { get; set; }
        public string? Observaciones { get; set; }
    }
}