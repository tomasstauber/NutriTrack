namespace NutriTrack.MAUI.Models
{
    // Respuesta de POST api/Login/login (en el back: LoginResponseDTO)
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        // Instante en UTC (llega terminado en "Z")
        public DateTime Expiracion { get; set; }

        public string NombreUsuario { get; set; } = string.Empty;

        // Se recibe como texto y no como RolUsuario a propósito: si llegara un
        // valor desconocido, el JSON fallaría con un error genérico. Así lo
        // convierte el AuthService y puede informar el problema con claridad.
        public string Rol { get; set; } = string.Empty;
    }
}
