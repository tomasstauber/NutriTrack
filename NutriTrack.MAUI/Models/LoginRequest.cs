namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Login/login (en el back: LoginDTO)
    public class LoginRequest
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;
    }
}
