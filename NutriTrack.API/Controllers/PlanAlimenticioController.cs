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
                NombreIngrediente = d.Ingrediente?.NombreIngrediente,   
                PorcentajeInclusionMs = d.PorcentajeInclusionMs,
                Observaciones = d.Observaciones
            }).ToList();

            return Ok(responseDto);
        }

        [HttpPut("{idPlanAlimenticio}")]
        public async Task<IActionResult> EditarPlanAlimenticioId(int idPlanAlimenticio, [FromBody] PlanAlimenticioDTO dto)
        {
            var plan = await _repository.BuscarPlanId(idPlanAlimenticio);
            if (plan is null)
            {
                return NotFound("No existe un plan con ese Id.");
            }

            // validar nombre unico excluyendo el propio plan
            bool nombreEnUso = await _repository.VerificarNombreUnicoExcluyendo(dto.NombrePlan, idPlanAlimenticio);
            if (nombreEnUso)
            {
                return BadRequest("Ya existe otro plan con ese nombre");
            }

            // validar que haya al menos un componente.
            if (dto.Detalle is null || !dto.Detalle.Any())
            {
                return BadRequest("El plan debe tener al menos un ingrediente.");
            }

            // validar ingredientes duplicados
            var idsIngredientes = dto.Detalle.Select(d => d.IdIngrediente).ToList();
            if (idsIngredientes.Count != idsIngredientes.Distinct().Count())
            {
                return BadRequest("No se puede repetir el mismo ingrediente en el plan.");
            }

            // validar suma de porcentajes
            var sumaPorcentaje = dto.Detalle.Sum(d => d.PorcentajeInclusionMs);
            if (sumaPorcentaje > 100)
            {
                return BadRequest("La suma de porcentajes de inclusión no puede superar 100.");
            }

            // validar ingresar un ingrediente no existente o desactivado
            foreach (var detalle in dto.Detalle)
            {
                bool existe = await _repository.ExisteIngrediente(detalle.IdIngrediente);
                if (!existe)
                {
                    return BadRequest($"El ingrediente con id {detalle.IdIngrediente} no existe o está desactivado.");
                }
            }

            // asignar los nuevos valores del DTO al plan
            plan.NombrePlan = dto.NombrePlan;
            plan.Categoria = dto.Categoria;
            plan.PesoVivoInicialPromedio = dto.PesoVivoInicialPromedio;
            plan.PesoObjetivo = dto.PesoObjetivo;
            plan.GananciaPesoEsperada = dto.GananciaPesoEsperada;
            plan.TipoAlimentacion = dto.TipoAlimentacion;
            plan.TiempoAlimentacion = dto.TiempoAlimentacion;
            plan.KgMsDiariaPorAnimal = dto.KgMsDiariaPorAnimal;
            plan.Observaciones = dto.Observaciones;

            // Limpiar detalles viejos y agregar los nuevos
            plan.Detalles.Clear();
            foreach (var detalleDTO in dto.Detalle)
            {
                plan.Detalles.Add(new PlanAlimenticioDetalle
                {
                    IdIngrediente = detalleDTO.IdIngrediente,
                    PorcentajeInclusionMs = detalleDTO.PorcentajeInclusionMs,
                    Observaciones = detalleDTO.Observaciones
                });
            }

            // guardar los cambios
            await _repository.ActualizarAsync(plan);

            // avisar de asignaciones vigentes
            int asignacionesAfectadas = await _repository.ContarAsignacionesVigentes(idPlanAlimenticio);
            if (asignacionesAfectadas > 0)
            {
                return Ok($"Plan actualizado. Atención los cambios afectan a {asignacionesAfectadas} rodeo(s) con asignación vigente");
            }

            return Ok("Plan actualizado exitosamente!");
        }
    }
}