using NutriTrack.Core.Entities;

namespace NutriTrack.API.DTOs
{
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public RolUsuario Rol { get; set; }
    }
}
