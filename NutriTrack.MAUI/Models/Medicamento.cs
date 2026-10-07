namespace NutriTrack.MAUI.Models
{
    // Medicamento del catálogo, tal como lo devuelve GET api/Medicamento
    // (NutriTrack.API.DTOs.MedicamentoResponseDTO).
    public class Medicamento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }

        // Texto de la columna Estado de la lista
        public string Estado => Activo ? "Activo" : "Inactivo";
    }
}
