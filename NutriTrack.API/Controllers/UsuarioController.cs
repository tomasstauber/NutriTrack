using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(UsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuarios()
        {
            var usuarios = await _usuarioRepository.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<IActionResult> CrearUsuario(CrearUsuarioDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre completo es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Correo))
                return BadRequest("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
                return BadRequest("El nombre de usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Contrasenia))
                return BadRequest("La contraseña es obligatoria.");

            if (dto.Contrasenia.Length < 8)
                return BadRequest("La contraseña debe tener al menos 8 caracteres.");

            var rolesValidos = new[]
            {
                "Administrador",
                "Encargado de campo",
                "Asesor técnico"
            };

            if (!rolesValidos.Contains(dto.Rol))
                return BadRequest("El rol seleccionado no es válido.");

            if (await _usuarioRepository.ExisteCorreoAsync(dto.Correo))
                return BadRequest("El correo electrónico ya está registrado.");

            if (await _usuarioRepository.ExisteNombreUsuarioAsync(dto.NombreUsuario))
                return BadRequest("El nombre de usuario ya está registrado.");

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Correo = dto.Correo,
                NombreUsuario = dto.NombreUsuario,
                Contrasenia = dto.Contrasenia,
                Rol = dto.Rol
            };

            await _usuarioRepository.CrearAsync(usuario);

            return Ok(usuario);
        }
    }
}