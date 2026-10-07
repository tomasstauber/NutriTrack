namespace NutriTrack.MAUI.Models
{
    // Cuerpo de POST api/Usuario (NutriTrack.API.DTOs.CrearUsuarioDTO)
    public class CrearUsuarioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string Contrasenia { get; set; } = string.Empty;

        // Viaja como texto con el nombre del enum ("Administrador", "EncargadoDeCampo"...)
        public RolUsuario Rol { get; set; }

        // R6: true solo si es Administrador y se aceptaron las dos confirmaciones
        public bool Confirmar { get; set; }
    }
}
