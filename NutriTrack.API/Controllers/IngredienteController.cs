using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredienteController : ControllerBase
    {
        private readonly IngredienteRepository _repository;

        public IngredienteController(IngredienteRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CrearIngrediente([FromBody] IngredienteDTO dto)
        {
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
                UnidadMedida = dto.UnidadMedida,
                Aditivos = dto.Aditivos
            };

            await _repository.CrearAsync(ingrediente);
            return Ok("Ingrediente creado exitosamente!");
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var ingrediente = await _repository.ObtenerTodosAsync();
            if (!ingrediente.Any())
            {
                return NotFound("No hay ingredientes almacenados.");
            }

            var responseDTO = ingrediente.Select(i => new IngredienteResponseDTO
            {
                NombreIngrediente = i.NombreIngrediente,
                Descripcion = i.Descripcion,
                Minerales = i.Minerales,
                EnergiaMetabolizable = i.EnergiaMetabolizable,
                ProteinaBruta = i.ProteinaBruta, 
                FibraDetergenteNeutro = i.FibraDetergenteNeutro,
                UnidadMedida = i.UnidadMedida,
                Aditivos = i.Aditivos
            }).ToList();

            return Ok(responseDTO);
        }

        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorNombre([FromQuery]string NombreIngrediente)
        {
            var ingredientes = await _repository.BuscarPorNombre(NombreIngrediente);
            if (!ingredientes.Any())
            {
                return NotFound("No existe un ingrediente con ese nombre.");
            }

            var responseDTO = ingredientes.Select(i => new IngredienteResponseDTO
            {
                NombreIngrediente = i.NombreIngrediente,
                Descripcion = i.Descripcion,
                Minerales = i.Minerales,
                EnergiaMetabolizable = i.EnergiaMetabolizable,
                ProteinaBruta = i.ProteinaBruta,
                FibraDetergenteNeutro = i.FibraDetergenteNeutro,
                UnidadMedida = i.UnidadMedida,
                Aditivos = i.Aditivos
            }).ToList();

            return Ok(responseDTO);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarIngrediente(int id, [FromBody] IngredienteDTO dto)
        {
            var ingrediente = await _repository.BuscarPorId(id);
            if (ingrediente is null)
            {
                return NotFound("No existe ningún ingrediente con ese Id.");
            }

            //validamos que el nombreno esté en uso
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
            ingrediente.UnidadMedida = dto.UnidadMedida;
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