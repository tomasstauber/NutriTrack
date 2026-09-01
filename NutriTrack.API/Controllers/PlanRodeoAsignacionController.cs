using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class PlanRodeoAsignacionController : ControllerBase
    {
        private readonly PlanRodeoAsignacionRepository _repository;
        private readonly PlanAlimenticioRepository _planAlimenticioRepository;
        private readonly AnimalRepository _animalRepository;
        private readonly RodeoRepository _rodeoRepository;
        public PlanRodeoAsignacionController(
            PlanRodeoAsignacionRepository repository,
            PlanAlimenticioRepository planRepository,
            AnimalRepository animalRepository,
            RodeoRepository rodeoRepository)
        {
            _repository = repository;
            _planAlimenticioRepository = planRepository;
            _animalRepository = animalRepository;
            _rodeoRepository = rodeoRepository;
        }

        [HttpPost]
        public async Task<IActionResult> AsignarPlanRodeo([FromBody] AsignarPlanDTO dto)
        {
            // Validar que el plan alimenticio exista
            var plan = await _planAlimenticioRepository.BuscarPlanId(dto.IdPlanAlimenticio);
            if (plan is null)
            {
                return NotFound("No existe un plan con ese Id.");
            }

            // Validar que el rodeo exista
            var rodeo = await _rodeoRepository.BuscarPorId(dto.IdRodeo);
            if (rodeo is null)
            {
                return NotFound("No existe un rodeo con ese Id.");
            }

            // Validar que el rodeo tenga al menos un animal activo asignado
            int cantidadAnimales = await _animalRepository.ContarActivosPorRodeo(dto.IdRodeo);
            if (cantidadAnimales == 0)
            {
                return BadRequest("El rodeo seleccionado no tiene animales activos asignados.");
            }

            // Validar coherencia de fechas de vigencia
            if (dto.VigenciaHasta.HasValue && dto.VigenciaHasta < dto.VigenciaDesde)
            {
                return BadRequest("La fecha de vigencia hasta no puede ser anterior a la fecha de inicio.");
            }

            // Si el rodeo tiene una asignación activa, cerrarla antes de crear la nueva
            string? planAnteriorReemplazado = null;
            var asignacionActiva = await _repository.ObtenerAsignacionActivaPorRodeo(dto.IdRodeo);

            //Si el plan ya está asignado a un rodeo, no se vuelve a asignar.
            if (asignacionActiva is not null && asignacionActiva.IdPlanAlimenticio == dto.IdPlanAlimenticio)
            {
                return BadRequest("El plan ya está asignado a este rodeo actualmente.");
            }

            if (asignacionActiva is not null)
            {
                asignacionActiva.Activo = false;
                asignacionActiva.VigenciaHasta = dto.VigenciaDesde;
                await _repository.ActualizarAsignacionAsync(asignacionActiva);
                planAnteriorReemplazado = asignacionActiva.PlanAlimenticio?.NombrePlan;
            }

            // Crear y persistir la nueva asignación
            var asignacion = new PlanRodeoAsignacion
            {
                VigenciaDesde = dto.VigenciaDesde,
                VigenciaHasta = dto.VigenciaHasta,
                Activo = true,
                IdRodeo = dto.IdRodeo,
                IdPlanAlimenticio = dto.IdPlanAlimenticio
            };

            await _repository.AsignarAsync(asignacion);

            // Calcular kg ms diaria total (cantidadAnimales ya se obtuvo en la validación de arriba)
            decimal kgMsDiariaTotal = plan.KgMsDiariaPorAnimal * cantidadAnimales;

            // Armar response
            var response = new AsignarPlanResponseDTO
            {
                NombrePlan = plan.NombrePlan,
                NombreRodeo = rodeo.Nombre,
                VigenciaDesde = dto.VigenciaDesde,
                VigenciaHasta = dto.VigenciaHasta,
                CantidadAnimales = cantidadAnimales,
                KgMsDiariaTotal = kgMsDiariaTotal,
                PlanAnteriorReemplazado = planAnteriorReemplazado
            };

            return Ok(response);
        }

        // Endpoint auxiliar para que el frontend consulte si un rodeo ya tiene plan activo
        // antes de mostrar el formulario de asignación. Permite advertir al usuario que se
        // va a reemplazar un plan existente antes de confirmar la operación. Solo lo pongo a modo de recordatorio.
        [HttpGet("rodeo/{idRodeo}/activa")]
        public async Task<IActionResult> ObtenerAsignacionActiva(int idRodeo)
        {
            var asignacion = await _repository.ObtenerAsignacionActivaPorRodeo(idRodeo);
            if (asignacion is null)
            {
                return NotFound("El rodeo no tiene un plan activo asignado.");
            }

            return Ok(new
            {
                NombrePlan = asignacion.PlanAlimenticio?.NombrePlan,
                VigenciaDesde = asignacion.VigenciaDesde
            });
        }
    }
}