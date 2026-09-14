using NutriTrack.Core.Entities;

namespace NutriTrack.API.DTOs
{
    public class CrearUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public string Contrasenia { get; set; }
        public RolUsuario Rol { get; set; }
        public bool Confirmar { get; set; }
    }

    public class EditarUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string NombreUsuario { get; set; }
        public RolUsuario Rol { get; set; }
        public bool Confirmar { get; set; }
    }
}