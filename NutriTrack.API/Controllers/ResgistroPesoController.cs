using Microsoft.AspNetCore.Mvc;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

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
        public async Task<IActionResult> Registrar([FromBody] RegistroPeso registro)
        {
            if (registro.PesoKg <= 0)
                return BadRequest("El peso debe ser mayor a cero.");

            if (registro.FechaPesaje > DateTime.Now)
                return BadRequest("La fecha no puede ser posterior a hoy.");

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