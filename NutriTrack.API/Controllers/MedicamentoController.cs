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
    public class MedicamentoController : ControllerBase
    {
        private readonly MedicamentoRepository _repository;

        public MedicamentoController(MedicamentoRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.AsesorTecnico}")]
        public async Task<IActionResult> CrearMedicamento([FromBody] MedicamentoDTO dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del medicamento es obligatorio.");

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
        public async Task<IActionResult> ObtenerTodosAsync(
            [FromQuery] string? nombre = null,
            [FromQuery] bool incluirInactivos = false)
        {
            if (incluirInactivos && !User.IsInRole(RolesUsuario.Administrador))
                return Forbid();

            var medicamento = await _repository.ObtenerTodosAsync(
                nombreMedicamento: nombre,
                incluirInactivos: incluirInactivos);

            var responseDTO = medicamento.Select(m => new MedicamentoResponseDTO
            {
                Id = m.Id,
                Nombre = m.Nombre,
                Descripcion = m.Descripcion,
                Activo = m.Activo
            })
                .OrderBy(m => m.Nombre)
                .ToList();

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
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.AsesorTecnico}")]
        public async Task<IActionResult> EditarMedicamento(int idMedicamento, [FromBody]MedicamentoDTO dto)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id o se encuentra desactivado.");
            }

            if (!medicamento.Activo)
            {
                return NotFound("El medicamento se encuentra desactivado.");
            }

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                return BadRequest("El nombre del medicamento es obligatorio.");

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
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.AsesorTecnico}")]
        public async Task<IActionResult> DesactivarMedicamento(int idMedicamento)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }

            if (!medicamento.Activo)
            {
                return Conflict("El medicamento ya está desactivado");
            }

            await _repository.DesactivarAsync(idMedicamento);
            return Ok("Medicamento desactivado correctamente.");
        }

        [HttpPatch("activar/{idMedicamento}")]
        [Authorize(Roles = RolesUsuario.Administrador)]
        public async Task<IActionResult> ActivarMedicamento(int idMedicamento)
        {
            var medicamento = await _repository.ObtenerPorIdAsync(idMedicamento);
            if (medicamento is null)
            {
                return NotFound("No existe un medicamento con ese Id.");
            }

            if (medicamento.Activo)
            {
                return Conflict("El medicamento ya está activo.");
            }

            await _repository.ActivarAsync(idMedicamento);
            return Ok("Medicamento activado correctamente.");
        }
    }
}