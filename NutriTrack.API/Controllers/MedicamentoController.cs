using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicamentoController : ControllerBase
    {
        private readonly MedicamentoRepository _repository;

        public MedicamentoController(MedicamentoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> CrearMedicamento([FromBody] MedicamentoDTO dto)
        {
            bool exists = await _repository.VerificarNombreUnico(dto.Nombre);
            if (exists)
            {
                return BadRequest("Ya existe un medicamento con ese nombre.");
            }

            var medicamento = new Medicamento
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };

            await _repository.AgregarAsync(medicamento);
            return Ok(new MedicamentoResponseDTO
            {
                Id = medicamento.Id,
                Nombre = medicamento.Nombre,
                Descripcion = medicamento.Descripcion,
                Activo = medicamento.Activo
            });
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodosAsync()
        {
            var medicamento = await _repository.ObtenerTodosAsync();
            if (!medicamento.Any())
            {
                return NotFound("No hay medicamentos almacenados.");
            }

            var responseDTO = medicamento.Select(m => new MedicamentoResponseDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activo = m.Activo
            }).ToList();

            return Ok(responseDTO);
        }

        [HttpGet("{idMedicamento}")]
        public async Task<IActionResult> BuscarMedicamentoPorId(int idMedicamento)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }

            var responseDTO = new MedicamentoResponseDTO
            {
                Id = medicamento.Id,
                Nombre = medicamento.Nombre,
                Descripcion = medicamento.Descripcion,
                Activo = medicamento.Activo
            };

            return Ok(responseDTO);
        }

        [HttpPut("{idMedicamento}")]
        public async Task<IActionResult> EditarMedicamento(int idMedicamento, [FromBody]MedicamentoDTO dto)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }

            bool nombreEnUso = await _repository.VerificarNombreUnicoExcluyendo(dto.Nombre, idMedicamento);
            if (nombreEnUso)
            {
                return BadRequest("Ya existe otro medicamento con ese nombre.");
            }

            medicamento.Nombre = dto.Nombre;
            medicamento.Descripcion = dto.Descripcion;
            await _repository.ActualizarAsync(medicamento);
            return Ok("Medicamento actualizado correctamente.");
        }

        [HttpDelete("{idMedicamento}")]
        public async Task<IActionResult> DesactivarMedicamento(int idMedicamento)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }

            await _repository.DesactivarAsync(idMedicamento);
            return Ok("Medicamento desactivado correctamente.");
        }

        [HttpPatch("activar/{idMedicamento}")]
        public async Task<IActionResult> ActivarMedicamento(int idMedicamento)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }
            await _repository.ActivarAsync(idMedicamento);
            return Ok("Medicamento activado correctamente.");
        }
    }
}