namespace NutriTrack.MAUI.Models
{
    // Resumen previo a eliminar un rodeo: GET api/EliminarRodeo/{idRodeo}
    // (NutriTrack.API.DTOs.EliminarRodeoResponseDTO)
    public class ResumenEliminacionRodeo
    {
        public string NombreRodeo { get; set; } = string.Empty;
        public int CantidadAnimales { get; set; }
        public int CantidadPlanes { get; set; }
    }
}
