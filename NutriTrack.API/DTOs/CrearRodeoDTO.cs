using System.Collections.Generic;

namespace NutriTrack.API.DTOs

{
    public class CrearRodeoDTO
    {
        public string Nombre { get; set; }
        public string? Descripcion { get; set; }
        public List<int> AnimalesIds { get; set; }
    }
}
