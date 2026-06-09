using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlanAlimenticioController : ControllerBase
    {
        private readonly PlanAlimenticioRepository _repository;

        public PlanAlimenticioController(PlanAlimenticioRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] PlanAlimenticioDTO dto)
        {
            bool exists = await _repository.VerificarNombreUnico(dto.NombrePlan);
            if (exists)
            {
                return BadRequest("Ya existe un plan con ese nombre.");
            }

            var plan = new PlanAlimenticio
            {
                NombrePlan = dto.NombrePlan,
                Categoria = dto.Categoria,
                PesoVivoInicialPromedio = dto.PesoVivoInicialPromedio,
                PesoObjetivo = dto.PesoObjetivo,
                GananciaPesoEsperada = dto.GananciaPesoEsperada,
                TipoAlimentacion = dto.TipoAlimentacion,
                TiempoAlimentacion = dto.TiempoAlimentacion,
                KgMsDiariaPorAnimal = dto.KgMsDiariaPorAnimal,
                Observaciones = dto.Observaciones,
            };

            plan.Detalles = dto.Detalle.Select(d => new PlanAlimenticioDetalle
            {
                IdIngrediente = d.IdIngrediente,
                PorcentajeInclusionMs = d.PorcentajeInclusionMs,
                Observaciones = d.Observaciones
            }).ToList();

            await _repository.CrearPlanAlimenticioAsync(plan);
            return Ok("Plan alimenticio creado exitosamente!");
        }

        [HttpGet("{idPlanAlimenticio}")]
        public async Task<IActionResult> BuscarPlanId(int idPlanAlimenticio)
        {
            var plan = await _repository.BuscarPlanId(idPlanAlimenticio);
            if (plan is null)
            {
                return NotFound("No existe un plan con ese Id.");
            }

            var responseDto = new PlanAlimenticioResponseDTO
            {
                NombrePlan = plan.NombrePlan,
                Categoria = plan.Categoria,
                PesoVivoInicialPromedio = plan.PesoVivoInicialPromedio,
                PesoObjetivo = plan.PesoObjetivo,
                GananciaPesoEsperada = plan.GananciaPesoEsperada,
                TipoAlimentacion = plan.TipoAlimentacion,
                TiempoAlimentacion = plan.TiempoAlimentacion,
                KgMsDiariaPorAnimal = plan.KgMsDiariaPorAnimal,
                Observaciones = plan.Observaciones
            };

            responseDto.Detalles = plan.Detalles.Select(d => new PlanAlimenticioDetalleResponseDTO
            {
                Id = d.Id,
                IdIngrediente = d.IdIngrediente,
                PorcentajeInclusionMs = d.PorcentajeInclusionMs,
                Observaciones = d.Observaciones
            }).ToList();
            
            return Ok(responseDto);
        }
    }
}