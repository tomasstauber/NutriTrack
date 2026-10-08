using NutriTrack.Core.Entities.Enums;

namespace NutriTrack.API.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public string Nombre { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }
    }
}
