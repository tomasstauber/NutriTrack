namespace NutriTrack.MAUI.Models
{
    // Último pesaje dentro de la ficha del animal (en el back: UltimoPesoDTO)
    public class UltimoPeso
    {
        // Id del pesaje
        public int Id { get; set; }
        public DateTime FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }
        public string? Observaciones { get; set; }
    }
}
