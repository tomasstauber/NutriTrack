namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Rodeo (en el back: CrearRodeoDTO)
    public class CrearRodeoRequest
    {
        // Ya recortado: el back no hace Trim
        public string Nombre { get; set; } = string.Empty;

        // Opcional: vacía viaja en null
        public string? Descripcion { get; set; }

        // Ids de los animales marcados en el selector (mínimo 2)
        public List<int> AnimalesIds { get; set; } = [];
    }
}
