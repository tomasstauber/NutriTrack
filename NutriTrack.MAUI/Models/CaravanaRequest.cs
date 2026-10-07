namespace NutriTrack.MAUI.Models
{
    // Caravana dentro de un pedido (en el back: CaravanaDTO).
    // El back la compara distinguiendo mayúsculas: se manda tal como la devolvió la API
    public class CaravanaRequest
    {
        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;
    }
}
