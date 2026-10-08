namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Medicamento y PUT api/Medicamento/{id} (NutriTrack.API.DTOs.MedicamentoDTO)
    public class MedicamentoRequest
    {
        public string Nombre { get; set; } = string.Empty;

        // Opcional: vacía viaja en null
        public string? Descripcion { get; set; }
    }
}
