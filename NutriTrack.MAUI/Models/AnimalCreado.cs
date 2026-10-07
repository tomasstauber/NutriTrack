namespace NutriTrack.MAUI.Models
{
    // Respuesta de POST api/Animal. El back devuelve un objeto anónimo con más datos:
    // se modela solo lo que usa el front
    public class AnimalCreado
    {
        // null si la respuesta no lo trae: en ese caso se vuelve a la lista
        public int? Id { get; set; }

        public string CaravanaCuig { get; set; } = string.Empty;
        public string CaravanaNroManejo { get; set; } = string.Empty;
    }
}
