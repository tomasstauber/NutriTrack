using Microsoft.AspNetCore.Mvc;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;
using NutriTrack.API.DTOs;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistroPesoController : ControllerBase
    {
        private readonly RegistroPesoRepository _repository;

        public RegistroPesoController(RegistroPesoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] RegistroPesoDTO dto)
        {
            if (dto.PesoKg <= 0)
                return BadRequest("El peso debe ser mayor a cero.");

            if (dto.FechaPesaje > DateTime.Now)
                return BadRequest("La fecha no puede ser posterior a hoy.");

            var registro = new RegistroPeso
            {
                FechaPesaje = dto.FechaPesaje,
                PesoKg = dto.PesoKg,
                Observaciones = dto.Observaciones,
                IdUsuario = dto.IdUsuario,
                IdAnimal = dto.IdAnimal
            };

            await _repository.AgregarAsync(registro);
            return Ok("Peso registrado exitosamente.");
        }

        [HttpGet("{idAnimal}")]
        public async Task<IActionResult> ObtenerPorAnimal(int idAnimal)
        {
            var registros = await _repository.ObtenerPorAnimalAsync(idAnimal);
            return Ok(registros);
        }
    }
}