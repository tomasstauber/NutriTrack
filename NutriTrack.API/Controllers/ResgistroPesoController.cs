using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.API.Helpers;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo}")]
    public class RegistroPesoController : ControllerBase
    {
        private readonly RegistroPesoRepository _repository;
        private readonly AnimalRepository _animalRepository;

        public RegistroPesoController(RegistroPesoRepository repository, AnimalRepository animalRepository)
        {
            _repository = repository;
            _animalRepository = animalRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistroPesoDTO dto)
        {
            if (dto.PesoKg <= 0)
                return BadRequest("El peso debe ser mayor a cero.");

            if (dto.FechaPesaje.Date > DateTime.Today)
                return BadRequest("La fecha no puede ser posterior a hoy.");

            var idUsuario = User.ObtenerId();

            var animal = await _animalRepository.ObtenerActivoPorId(dto.IdAnimal);

            if (animal is null)
                return BadRequest("No se encontró un animal activo con esa caravana");

            var registro = new RegistroPeso
            {
                FechaPesaje = dto.FechaPesaje,
                PesoKg = dto.PesoKg,
                Observaciones = dto.Observaciones,
                IdUsuario = idUsuario,
                IdAnimal = dto.IdAnimal
            };

            await _repository.AgregarAsync(registro);

            var registroResponse = new RegistroPesoResponseDTO
            {
                Id = registro.Id,
                FechaPesaje = registro.FechaPesaje,
                PesoKg = registro.PesoKg,
                Observaciones = registro.Observaciones
            };

            return Ok(registroResponse);
        }

        [HttpGet("{idAnimal}")]
        public async Task<IActionResult> ObtenerPorAnimal(int idAnimal)
        {
            var registros = await _repository.ObtenerPorAnimalAsync(idAnimal);

            var response = registros
                .Select(r => new RegistroPesoResponseDTO
                {
                    Id = r.Id,
                    FechaPesaje = r.FechaPesaje,
                    PesoKg = r.PesoKg,
                    Observaciones = r.Observaciones
                })
                .ToList();

            return Ok(response);
        }
    }
}