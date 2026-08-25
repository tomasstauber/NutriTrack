namespace NutriTrack.API.DTOs
{
    public class CrearUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenia { get; set; }
        public NutriTrack.Core.Entities.RolUsuario Rol { get; set; }
    }

    public class EditarUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public NutriTrack.Core.Entities.RolUsuario Rol { get; set; }
    }
}