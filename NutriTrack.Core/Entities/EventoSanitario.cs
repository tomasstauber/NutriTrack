namespace NutriTrack.Core.Entities
{
    public class EventoSanitario
    {
        public int Id { get; set; }
        public string? TipoDeEvento { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaEvento { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public int IdUsuario { get; set; }
        public int IdAnimal { get; set; }
    }
}