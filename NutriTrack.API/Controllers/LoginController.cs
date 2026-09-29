using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.API.Services;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly TokenService _tokenService;

        public LoginController(UsuarioRepository usuarioRepository, TokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var usuario = await _usuarioRepository.ValidarCredencialesAsync(dto.NombreUsuario, dto.Contrasenia);

            if (usuario == null)
            {
                return Unauthorized(new {mensaje = "Credenciales inválidas"});
            }

            // tupla desarmada
            var (token, expiracion) = _tokenService.GenerarToken(usuario);

            var respuesta = new LoginResponseDTO
            {
                Token = token,
                Expiracion = expiracion,
                NombreUsuario = usuario.NombreUsuario,
                Rol = usuario.Rol
            };

            return Ok(respuesta);
        }
    }
}
