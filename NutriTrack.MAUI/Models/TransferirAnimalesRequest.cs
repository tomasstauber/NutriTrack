namespace NutriTrack.MAUI.Models
{
    // Cuerpo de PATCH api/TransferenciaAnimal (en el back: TransferenciaAnimalesDTO)
    public class TransferirAnimalesRequest
    {
        public int IdRodeoOrigen { get; set; }

        // Distinto del origen (R3)
        public int IdRodeoDestino { get; set; }

        // Ids de los animales marcados en el selector (al menos uno)
        public List<int> AnimalesIds { get; set; } = [];
    }
}
