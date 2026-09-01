using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransferenciaAnimalController : ControllerBase
    {
        private readonly TransferenciaAnimalesRepository _repository;
        private readonly RodeoRepository _rodeoRepository;
        public TransferenciaAnimalController(TransferenciaAnimalesRepository repository, RodeoRepository rodeoRepository)
        {
            _repository = repository;
            _rodeoRepository = rodeoRepository;

        }
        //Rodeo origen debe existir
        [HttpPatch]
        public async Task<IActionResult> Transferir([FromBody] TransferenciaAnimalesDTO dto)
        {
            // 1. Rodeo origen existe
            var rodeoOrigen = await _rodeoRepository.BuscarPorId(dto.IdRodeoOrigen);
            if (rodeoOrigen == null)
                return NotFound("No existe el rodeo origen.");

            // Rodeo origen debe tener al menos un animal activo
            var animalesOrigen = await _repository.ObtenerAnimalesPorRodeo(dto.IdRodeoOrigen);
            if (!animalesOrigen.Any())
                return BadRequest("El rodeo origen no tiene animales activos.");

            // 2. Rodeo destino existe
            var rodeoDestino = await _rodeoRepository.BuscarPorId(dto.IdRodeoDestino);
            if (rodeoDestino == null)
                return NotFound("No existe el rodeo destino.");

            // 3. Rodeos distintos
            if (dto.IdRodeoOrigen == dto.IdRodeoDestino)
                return BadRequest("El rodeo destino debe ser distinto al rodeo origen.");

            // 4. Al menos un animal seleccionado
            if (dto.AnimalesIds == null || dto.AnimalesIds.Count == 0)
                return BadRequest("Debe seleccionar al menos un animal.");

            // 5. Validar cada animal y acumular los que se van a transferir
            var animalesATransferir = new List<Animal>();
            foreach (var idAnimal in dto.AnimalesIds)
            {
                var animal = await _repository.ObtenerAnimalPorId(idAnimal);
                if (animal == null)
                    return BadRequest($"No existe el animal con ID {idAnimal}.");

                if (animal.RodeoId != dto.IdRodeoOrigen)
                    return BadRequest($"El animal {idAnimal} no pertenece al rodeo origen.");

                animalesATransferir.Add(animal);
            }

            // 6. Transferir — operacion atomica
            foreach (var animal in animalesATransferir)
            {
                animal.RodeoId = dto.IdRodeoDestino;
            }
            await _repository.GuardarCambios();

            return Ok(new
            {
                CantidadAnimalesTransferidos = animalesATransferir.Count,
                NombreRodeoOrigen = rodeoOrigen.Nombre,
                NombreRodeoDestino = rodeoDestino.Nombre,
                AnimalesTransferidos = animalesATransferir.Select(a => new
                {
                    a.Id,
                    a.CaravanaCuig,
                    a.CaravanaNroManejo
                })
            });
        }

        // GET: traer animales del rodeo origen
        [HttpGet("rodeo/{idRodeo}/animales")]
        public async Task<IActionResult> ObtenerAnimalesDeRodeo(int idRodeo)
        {
            var rodeo = await _rodeoRepository.BuscarPorId(idRodeo);
            if (rodeo == null)
                return NotFound("No existe el rodeo.");

            var animales = await _repository.ObtenerAnimalesPorRodeo(idRodeo);
            if (!animales.Any())
                return NotFound("El rodeo no tiene animales.");

            return Ok(animales.Select(a => new TransferenciaAnimalResponseDTO
            {
                Id = a.Id,
                CaravanaCuig = a.CaravanaCuig,
                CaravanaNroManejo = a.CaravanaNroManejo
            }));
        }
    }
}
