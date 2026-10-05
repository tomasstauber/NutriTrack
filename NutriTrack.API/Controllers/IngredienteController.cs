using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Core.Entities.Enums;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.AsesorTecnico}")]
    public class IngredienteController : ControllerBase
    {
        private readonly IngredienteRepository _repository;

        public IngredienteController(IngredienteRepository repository)
        {
            _repository = repository;
        }

        // Arma el DTO de respuesta. Se usa en todos los endpoints que devuelven ingredientes.
        private static IngredienteResponseDTO ADto(Ingrediente i)
        {
            return new IngredienteResponseDTO
            {
                Id = i.Id,
                NombreIngrediente = i.NombreIngrediente,
                Descripcion = i.Descripcion,
                Minerales = i.Minerales,
                EnergiaMetabolizable = i.EnergiaMetabolizable,
                ProteinaBruta = i.ProteinaBruta,
                FibraDetergenteNeutro = i.FibraDetergenteNeutro,
                UnidadMedida = i.UnidadMedida,
                Aditivos = i.Aditivos
            };
        }

        // Validaciones compartidas por alta y edición. Devuelve el mensaje de error, o null si está todo bien.
        private static string? ValidarIngrediente(IngredienteDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreIngrediente))
                return "El nombre del ingrediente es obligatorio.";

            if (dto.EnergiaMetabolizable < 0 || dto.ProteinaBruta < 0 || dto.FibraDetergenteNeutro < 0)
                return "Los valores nutricionales deben ser mayores o iguales a 0.";

            if (!Enum.TryParse<UnidadMedida>(dto.UnidadMedida, ignoreCase: true, out var unidad)
                || !Enum.IsDefined(unidad))
                return $"Unidad de medida inválida. Opciones: {string.Join(", ", Enum.GetNames<UnidadMedida>())}.";

            return null;
        }

        // Devuelve la unidad escrita como en el enum (por ejemplo "KG" pasa a "kg").
        private static string NormalizarUnidad(string unidad)
        {
            return Enum.Parse<UnidadMedida>(unidad, ignoreCase: true).ToString();
        }

        [HttpPost]
        public async Task<IActionResult> CrearIngrediente([FromBody] IngredienteDTO dto)
        {
            var error = ValidarIngrediente(dto);
            if (error is not null)
                return BadRequest(error);

            bool exists = await _repository.VerificarNombreUnico(dto.NombreIngrediente);
            if (exists)
            {
                return BadRequest("Ya existe un ingrediente con ese nombre.");
            }

            var ingrediente = new Ingrediente
            {
                NombreIngrediente = dto.NombreIngrediente,
                Descripcion = dto.Descripcion,
                Minerales = dto.Minerales,
                EnergiaMetabolizable = dto.EnergiaMetabolizable,
                ProteinaBruta = dto.ProteinaBruta,
                FibraDetergenteNeutro = dto.FibraDetergenteNeutro,
                UnidadMedida = NormalizarUnidad(dto.UnidadMedida),
                Aditivos = dto.Aditivos
            };

            await _repository.CrearAsync(ingrediente);
            return Ok(ADto(ingrediente));
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var ingredientes = await _repository.ObtenerTodosAsync();
            return Ok(ingredientes.Select(ADto).ToList());
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorNombre([FromQuery] string NombreIngrediente)
        {
            var ingredientes = await _repository.BuscarPorNombre(NombreIngrediente);
            return Ok(ingredientes.Select(ADto).ToList());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarIngrediente(int id, [FromBody] IngredienteDTO dto)
        {
            var ingrediente = await _repository.BuscarPorId(id);
            if (ingrediente is null)
            {
                return NotFound("No existe ningún ingrediente con ese Id.");
            }

            if (!ingrediente.Activo)
            {
                return NotFound("No existe ningún ingrediente con ese Id.");
            }

            var error = ValidarIngrediente(dto);
            if (error is not null)
                return BadRequest(error);

            // validamos que el nombre no esté en uso
            if (ingrediente.NombreIngrediente.ToLower() != dto.NombreIngrediente.ToLower())
            {
                bool exist = await _repository.VerificarNombreUnico(dto.NombreIngrediente);
                if (exist)
                {
                    return BadRequest("Ya existe un ingrediente con ese nombre.");
                }
            }

            ingrediente.NombreIngrediente = dto.NombreIngrediente;
            ingrediente.Descripcion = dto.Descripcion;
            ingrediente.Minerales = dto.Minerales;
            ingrediente.EnergiaMetabolizable = dto.EnergiaMetabolizable;
            ingrediente.ProteinaBruta = dto.ProteinaBruta;
            ingrediente.FibraDetergenteNeutro = dto.FibraDetergenteNeutro;
            ingrediente.UnidadMedida = NormalizarUnidad(dto.UnidadMedida);
            ingrediente.Aditivos = dto.Aditivos;

            await _repository.ActualizarAsync(ingrediente);
            return Ok("Ingrediente actualizado exitosamente!");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarIngrediente(int id)
        {
            var ingrediente = await _repository.BuscarPorId(id);
            if (ingrediente is null)
            {
                return NotFound("No existe un ingrediente con ese Id.");
            }

            if (!ingrediente.Activo)
            {
                return Conflict("El ingrediente ya se encuentra desactivado.");
            }

            var planesQueLoUsan = await _repository.ObtenerPlanesQueUsan(id);

            await _repository.DesactivarAsync(ingrediente);

            if (planesQueLoUsan.Any())
            {
                var nombres = string.Join(", ", planesQueLoUsan.Select(p => p.NombrePlan));
                return Ok($"Ingrediente desactivado. Atención: está siendo usado en los siguientes planes: {nombres}");
            }

            return Ok("Ingrediente desactivado exitosamente!");
        }
    }
}