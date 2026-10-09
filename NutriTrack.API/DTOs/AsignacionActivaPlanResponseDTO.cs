namespace NutriTrack.API.DTOs
{
    public class AsignacionActivaPlanResponseDTO
    {
        public int IdAsignacion { get; set; }
        public int IdRodeo { get; set; }
        public string NombreRodeo { get; set; }
        public string? DescripcionRodeo { get; set; }
        public int CantidadAnimales { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
    }
}
