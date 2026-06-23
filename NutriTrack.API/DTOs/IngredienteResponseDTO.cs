namespace NutriTrack.API.DTOs
{
    public class IngredienteResponseDTO
    {
        public string NombreIngrediente { get; set; }
        public string? Descripcion { get; set; }
        public string? Minerales { get; set; }
        public decimal? EnergiaMetabolizable { get; set; }
        public decimal? ProteinaBruta { get; set; }
        public decimal? FibraDetergenteNeutro { get; set; }
        public string UnidadMedida { get; set; }
        public string? Aditivos { get; set; }
    }
}
