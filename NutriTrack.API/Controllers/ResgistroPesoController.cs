using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
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