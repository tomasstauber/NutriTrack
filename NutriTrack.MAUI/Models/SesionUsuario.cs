namespace NutriTrack.MAUI.Models
{
    // Datos del usuario logueado. Se arman a partir de la respuesta
    // de POST api/login y se guardan mientras la sesión esté vigente.
    public class SesionUsuario
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }

        public bool EstaVigente => Expiracion > DateTime.UtcNow;
    }
}