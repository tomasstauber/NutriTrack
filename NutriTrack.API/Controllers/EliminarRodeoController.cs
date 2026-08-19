using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EliminarRodeoController : ControllerBase
    {
        private readonly EliminarRodeoRepository _repository;
        private readonly RodeoRepository _rodeoRepository;

        private readonly AnimalRepository _animalRepository;

        public EliminarRodeoController(EliminarRodeoRepository repository, RodeoRepository rodeoRepository, AnimalRepository animalRepository)
        {
            _repository = repository;
            _rodeoRepository = rodeoRepository;
            _animalRepository = animalRepository;
        }

        [HttpGet("{idRodeo}")]
        public async Task<IActionResult> ObtenerResumen(int idRodeo)
        {
            var rodeo = await _rodeoRepository.BuscarPorId(idRodeo);
            if (rodeo == null)
                return NotFound("No existe el rodeo.");

            var cantidadAnimales = await _animalRepository.ContarActivosPorRodeo(idRodeo);
            var cantidadPlanes = await _repository.ContarPlanesPorRodeo(idRodeo);

            return Ok( new EliminarRodeoResponseDTO
            {
                NombreRodeo = rodeo.Nombre,
                CantidadAnimales = cantidadAnimales,
                CantidadPlanes= cantidadPlanes
            });
        }

        [HttpDelete("{idRodeo}")]
        public async Task<IActionResult> EliminarRodeo(int idRodeo, [FromBody] EliminarRodeoDTO dto)
        {
            var rodeo = await _rodeoRepository.BuscarPorId(idRodeo);
            if (rodeo == null)
                return NotFound("No existe el rodeo.");

            if (!dto.Confirmar)
                return BadRequest("Debe confirmar la eliminación.");

            await _repository.EliminarRodeo(idRodeo);
            return Ok("Rodeo eliminado correctamente.");
        }
    }
}
