using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RodeoController : ControllerBase
    {
        private readonly CrearRodeoRepository _rodeoRepo;
        private readonly AnimalRepository _animalRepo;

        public RodeoController(CrearRodeoRepository rodeoRepo, AnimalRepository animalRepo)
        {
            _rodeoRepo = rodeoRepo;
            _animalRepo = animalRepo;
        }

        // GET: api/rodeo/animales-disponibles?caravana=123
        [HttpGet("animales-disponibles")]
        public IActionResult ObtenerAnimalesDisponibles([FromQuery] string? caravana)
        {
            var animales = _animalRepo.ObtenerActivosSinRodeo(caravana);
            return Ok(animales.Select(a => new {
                a.Id,
                a.CaravanaCuig,
                a.CaravanaNroManejo
            }));
        }
        // POST: api/rodeo
        [HttpPost]
        public IActionResult Crear([FromBody] CrearRodeoDTO dto)
        {
            // Validar mínimo 2 animales
            if (dto.AnimalesIds == null || dto.AnimalesIds.Count < 2)
                return BadRequest("Debe seleccionar al menos 2 animales.");

            // Validar nombre único
            if (_rodeoRepo.ExisteNombre(dto.Nombre))
                return Conflict("Ya existe un rodeo con ese nombre.");

            // Validar que los animales existan, estén activos y sin rodeo
            var animales = _animalRepo.ObtenerPorIds(dto.AnimalesIds);
            if (animales.Count != dto.AnimalesIds.Count)
                return BadRequest("Uno o más animales no están disponibles para asignar.");

            // Crear rodeo y asignar animales
            var rodeo = new Rodeo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Animales = animales
            };

            _rodeoRepo.Crear(rodeo);

            return Ok(new RodeoResponseDTO
            {
                Id = rodeo.Id,
                NombreRodeo = rodeo.Nombre,
                CantidadAnimales = rodeo.Animales.Count
            });
        }
    }
}