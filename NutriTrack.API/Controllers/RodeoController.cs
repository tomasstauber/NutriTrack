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
    public class RodeoController : ControllerBase
    {
        private readonly RodeoRepository _rodeoRepo;
        private readonly AnimalRepository _animalRepo;

        public RodeoController(RodeoRepository rodeoRepo, AnimalRepository animalRepo)
        {
            _rodeoRepo = rodeoRepo;
            _animalRepo = animalRepo;
        }

        [HttpGet]
        public async Task<IActionResult> ListarRodeos()
        {
            var rodeos = await _rodeoRepo.ListarRodeosActivos();
            return Ok(rodeos.Select(r => new RodeoListadoResponseDTO
            {
                Id = r.Id,
                Nombre = r.Nombre,
                Descripcion = r.Descripcion,
                CantidadAnimales = r.CantidadAnimales
            }).ToList());
        }

        // POST: api/rodeo
        [HttpPost]
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo}")]
        public async Task<IActionResult> Crear([FromBody] CrearRodeoDTO dto)
        {
            //validar que no sea null 
            if (string.IsNullOrEmpty(dto.Nombre))
                return BadRequest("El nombre del rodeo es obligatorio.");

            // Validar mínimo 2 animales
            if (dto.AnimalesIds == null || dto.AnimalesIds.Count < 2)
                return  BadRequest("Debe seleccionar al menos 2 animales.");

            // Validar nombre único
            if (await _rodeoRepo.ExisteNombre(dto.Nombre))
                return Conflict("Ya existe un rodeo con ese nombre.");

            // Validar que los animales existan, estén activos y sin rodeo
            var animales = await _animalRepo.ObtenerPorIds(dto.AnimalesIds);
            if (animales.Count != dto.AnimalesIds.Count)
                return BadRequest("Uno o más animales no están disponibles para asignar.");

            // Crear rodeo y asignar animales
            var rodeo = new Rodeo
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Animales = animales
            };

            await _rodeoRepo.Crear(rodeo);

            return Ok(new RodeoResponseDTO
            {
                Mensaje = "Rodeo creado con éxito.",
                Id = rodeo.Id,
                NombreRodeo = rodeo.Nombre,
                Descripcion = rodeo.Descripcion,
                CantidadAnimales = rodeo.Animales.Count
            });
        }
    }
}