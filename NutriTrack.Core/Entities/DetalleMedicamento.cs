namespace NutriTrack.Core.Entities
{
    public class DetalleMedicamento
    {
        public int Id { get; set; }

        public float Dosis { get; set; }

        public string? Unidad { get; set; }

        public int IdEventoSanitario { get; set; }

        public int IdMedicamento { get; set; }

        public EventoSanitario? EventoSanitario { get; set; }

        public Medicamento? Medicamento { get; set; }
    }
}