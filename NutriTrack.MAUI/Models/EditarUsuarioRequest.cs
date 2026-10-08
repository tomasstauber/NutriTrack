namespace NutriTrack.MAUI.Models
{
    // Cuerpo de PUT api/Usuario/{id} (NutriTrack.API.DTOs.EditarUsuarioDTO). No lleva contraseña
    public class EditarUsuarioRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;

        // Viaja como texto con el nombre del enum ("Administrador", "EncargadoDeCampo"...)
        public RolUsuario Rol { get; set; }

        // R6: true solo si el rol pasa a Administrador y se aceptaron las dos confirmaciones
        public bool Confirmar { get; set; }
    }
}
