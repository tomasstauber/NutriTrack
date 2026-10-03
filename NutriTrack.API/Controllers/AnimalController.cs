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
    public class AnimalController : ControllerBase
    {
        private const int LargoMaximoParteCaravana = 5;
        private const string MensajeCaravanaInvalida =
            "Formato de caravana inválido (cada parte alfanumérica, hasta 5 caracteres).";

        private readonly AltaAnimalRepository _animalRepo;
        private readonly DesactivacionReactivacionAnimalRepository _desactivacionRepo;
        private readonly AnimalRepository _listadoRepo;

        public AnimalController(
            AltaAnimalRepository animalRepo,
            DesactivacionReactivacionAnimalRepository desactivacionRepo,
            AnimalRepository listadoRepo)
        {
            _animalRepo = animalRepo;
            _desactivacionRepo = desactivacionRepo;
            _listadoRepo = listadoRepo;
        }
        private static bool ParteCaravanaValida(string? parte)
        {
            return !string.IsNullOrEmpty(parte)
                && parte.Length <= LargoMaximoParteCaravana
                && parte.All(char.IsLetterOrDigit);
        }

        // Listado de animales con filtros y paginación.
        [HttpGet]
        [ProducesResponseType(typeof(ListadoPaginadoDTO<AnimalListadoDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ListadoPaginadoDTO<AnimalListadoDTO>>> Listar(
            [FromQuery] string? texto,
            [FromQuery] int? idRodeo,
            [FromQuery] bool sinRodeo = false,
            [FromQuery] bool incluirInactivos = false,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanioPagina = 50)
        {
            if (pagina < 1)
                return BadRequest("La página debe ser mayor o igual a 1.");

            if (tamanioPagina < 1 || tamanioPagina > 100)
                return BadRequest("El tamaño de página debe estar entre 1 y 100.");

            if (sinRodeo && idRodeo.HasValue)
                return BadRequest("No se puede filtrar por rodeo y por animales sin rodeo a la vez.");

            var (animales, total) = await _listadoRepo.Listar(
                texto, idRodeo, sinRodeo, incluirInactivos, pagina, tamanioPagina);

            var respuesta = new ListadoPaginadoDTO<AnimalListadoDTO>
            {
                Items = animales.Select(a => new AnimalListadoDTO
                {
                    Id = a.Id,
                    CaravanaCuig = a.CaravanaCuig,
                    CaravanaNroManejo = a.CaravanaNroManejo,
                    Raza = a.Raza,
                    Sexo = a.Sexo.ToString(),
                    FechaNacimiento = a.FechaNacimiento,
                    Estado = a.Estado,
                    RodeoId = a.RodeoId,
                    RodeoNombre = a.Rodeo?.Nombre
                }).ToList(),
                Total = total,
                Pagina = pagina,
                TamanioPagina = tamanioPagina
            };

            return Ok(respuesta);
        }

        [HttpPost]
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo}")]
        public async Task<IActionResult> Crear([FromBody] CrearAnimalDTO dto)
        {
            if (!ParteCaravanaValida(dto.CaravanaCuig) || !ParteCaravanaValida(dto.CaravanaNroManejo))
                return BadRequest(MensajeCaravanaInvalida);

            if (await _animalRepo.ExisteCaravana(dto.CaravanaCuig, dto.CaravanaNroManejo))
                return Conflict("Ya existe un animal con esa caravana.");

            if (dto.PesoAlNacer <= 0 || dto.PesoAlNacer > 100)
                return BadRequest("El peso al nacer debe ser mayor a 0 y menor o igual a 100 kg.");

            if (dto.FechaNacimiento > DateTime.Now)
                return BadRequest("La fecha de nacimiento no puede ser posterior a hoy.");

            Animal? madre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigMadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoMadre))
            {
                madre = await _animalRepo.BuscarPorCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre);
                if (madre == null)
                    return BadRequest("No se encontró un animal con la caravana de la madre indicada.");
            }

            Animal? padre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigPadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoPadre))
            {
                padre = await _animalRepo.BuscarPorCaravana(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre);
                if (padre == null)
                    return BadRequest("No se encontró un animal con la caravana del padre indicada.");
            }

            var animal = new Animal
            {
                CaravanaCuig = dto.CaravanaCuig,
                CaravanaNroManejo = dto.CaravanaNroManejo,
                FechaNacimiento = dto.FechaNacimiento,
                PesoAlNacer = dto.PesoAlNacer,
                MadreId = madre?.Id,
                PadreId = padre?.Id,
                Raza = dto.Raza,
                Sexo = dto.Sexo,
                ColorPelaje = dto.ColorPelaje,
                FechaAlta = DateTime.Now, // lo asigna automáticamente el sistema
                Estado = true,            // activo por defecto
                RodeoId = null            // sin rodeo al dar de alta
            };

            await _animalRepo.Crear(animal);

            return Ok(new
            {
                animal.Id,
                animal.CaravanaCuig,
                animal.CaravanaNroManejo,
                animal.FechaNacimiento,
                animal.PesoAlNacer,
                animal.Raza,
                Sexo = animal.Sexo.ToString(),
                animal.ColorPelaje,
                animal.FechaAlta,
                animal.Estado,
                Madre = madre != null ? $"{madre.CaravanaCuig}-{madre.CaravanaNroManejo}" : null,
                Padre = padre != null ? $"{padre.CaravanaCuig}-{padre.CaravanaNroManejo}" : null
            });
        }

        [HttpPatch("desactivar")]
        [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo}")]
        public async Task<IActionResult> Desactivar([FromQuery] string cuig, [FromQuery] string nroManejo)
        {
            if (!ParteCaravanaValida(cuig) || !ParteCaravanaValida(nroManejo))
                return BadRequest(MensajeCaravanaInvalida);

            var animal = await _desactivacionRepo.BuscarPorCaravana(cuig, nroManejo);
            if (animal == null)
                return NotFound("No se encontró un animal con esa caravana");

            if (!animal.Estado)
                return BadRequest("El animal ya está inactivo.");

            animal.Estado = false;
            await _desactivacionRepo.Actualizar(animal);
            return Ok("Animal desactivado correctamente.");
        }

        [HttpPatch("reactivar")]
        [Authorize(Roles = RolesUsuario.Administrador)]
        public async Task<IActionResult> Reactivar([FromQuery] string cuig, [FromQuery] string nroManejo)
        {
            if (!ParteCaravanaValida(cuig) || !ParteCaravanaValida(nroManejo))
                return BadRequest(MensajeCaravanaInvalida);

            var animal = await _desactivacionRepo.BuscarPorCaravana(cuig, nroManejo);
            if (animal == null)
                return NotFound("No se encontró un animal con esa caravana");

            if (animal.Estado)
                return BadRequest("El animal ya está activo.");

            animal.Estado = true;
            await _desactivacionRepo.Actualizar(animal);
            return Ok("Animal reactivado correctamente.");
        }
    }
}