namespace NutriTrack.Core.Entities
{
    public class Usuario
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public string Correo { get; set; }

        public string NombreUsuario { get; set; }

        public RolUsuario Rol { get; set; }

        public string Contrasenia { get; set; }

        public bool Activo { get; set; } = true;
    }
}