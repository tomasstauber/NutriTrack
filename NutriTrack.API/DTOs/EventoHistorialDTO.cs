namespace NutriTrack.API.DTOs
{
    public class EventoHistorialDTO
    {
        public int Id { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
        public string? Responsable { get; set; }
        public List<MedicamentoHistorialDTO> Medicamentos { get; set; } = new();
    }

    public class MedicamentoHistorialDTO
    {
        public string? Nombre { get; set; }
        public decimal? Dosis { get; set; }
        public string? Unidad { get; set; }
        public string? Observaciones { get; set; }
    }
}