namespace NutriTrack.MAUI.Models
{
    // Respuesta 200 de PATCH api/TransferenciaAnimal. El back devuelve un objeto anónimo
    // que además trae la lista de animales transferidos: se modela solo lo que usa el front
    public class TransferenciaResponse
    {
        public int CantidadAnimalesTransferidos { get; set; }
        public string NombreRodeoOrigen { get; set; } = string.Empty;
        public string NombreRodeoDestino { get; set; } = string.Empty;
    }
}
