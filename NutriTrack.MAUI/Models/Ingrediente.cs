namespace NutriTrack.MAUI.Models
{
    // Ingrediente del catálogo, tal como lo devuelve GET api/Ingrediente
    // (NutriTrack.API.DTOs.IngredienteResponseDTO). Solo llegan los activos.
    public class Ingrediente
    {
        public int Id { get; set; }
        public string NombreIngrediente { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? Minerales { get; set; }
        public decimal? EnergiaMetabolizable { get; set; }
        public decimal? ProteinaBruta { get; set; }
        public decimal? FibraDetergenteNeutro { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public string? Aditivos { get; set; }
    }
}
