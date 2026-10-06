namespace NutriTrack.MAUI.Models
{
    // Último pesaje dentro de la ficha del animal (en el back: UltimoPesoDTO)
    public class UltimoPeso
    {
        // OJO: trae el id del ANIMAL, no el del pesaje (el back hace Id = animal.Id).
        // No usarlo como id de pesaje.
        public int Id { get; set; }
        public DateTime FechaPesaje { get; set; }
        public decimal PesoKg { get; set; }
    }
}
