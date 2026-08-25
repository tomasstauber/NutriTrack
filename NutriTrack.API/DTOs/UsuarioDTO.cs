namespace NutriTrack.API.DTOs
{
    using NutriTrack.Core.Entities;

    public class CrearUsuarioDTO
    {
        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string NombreUsuario { get; set; }

        public string Contrasenia { get; set; }

        public RolUsuario Rol { get; set; }
    }
}