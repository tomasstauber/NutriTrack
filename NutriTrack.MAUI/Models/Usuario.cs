namespace NutriTrack.MAUI.Models
{
    // Usuario tal como lo devuelve GET api/Usuario
    // (NutriTrack.API.DTOs.UsuarioResponseDTO). Solo llegan los activos.
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public RolUsuario Rol { get; set; }

        // No viene de la API: el rol en texto legible para la tabla
        public string RolTexto => Rol switch
        {
            RolUsuario.Administrador => "Administrador",
            RolUsuario.EncargadoDeCampo => "Encargado de campo",
            RolUsuario.AsesorTecnico => "Asesor técnico",
            _ => Rol.ToString()
        };
    }
}
