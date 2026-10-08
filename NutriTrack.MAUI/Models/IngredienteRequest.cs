namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Ingrediente (NutriTrack.API.DTOs.IngredienteDTO)
    public class IngredienteRequest
    {
        public string NombreIngrediente { get; set; } = string.Empty;

        // Opcionales: vacíos viajan en null
        public string? Descripcion { get; set; }
        public string? Minerales { get; set; }

        // Opcionales: vacíos viajan en null, no en 0
        public decimal? EnergiaMetabolizable { get; set; }
        public decimal? ProteinaBruta { get; set; }
        public decimal? FibraDetergenteNeutro { get; set; }

        // Texto exacto del enum UnidadMedida del back, en minúscula ("kg", "fardo"...)
        public string UnidadMedida { get; set; } = string.Empty;

        public string? Aditivos { get; set; }
    }
}
