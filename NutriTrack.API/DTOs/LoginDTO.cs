using System.ComponentModel.DataAnnotations;
using static NutriTrack.API.Constants.LimitesUsuario;

namespace NutriTrack.API.DTOs
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(NombreUsuarioMax)]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required]
        [MinLength(ContraseniaMin)]
        [MaxLength(ContraseniaMax)]
        public string Contrasenia { get; set; } = string.Empty;
    }
}