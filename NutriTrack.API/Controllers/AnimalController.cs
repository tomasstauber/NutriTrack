using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.API.Helpers;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
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
            if (!CaravanaHelper.ParteValida(dto.CaravanaCuig) || !CaravanaHelper.ParteValida(dto.CaravanaNroManejo))
                return BadRequest(CaravanaHelper.MensajeCaravanaInvalida);

            if (await _animalRepo.ExisteCaravana(dto.CaravanaCuig, dto.CaravanaNroManejo))
                return Conflict("Ya existe un animal con esa caravana.");

            if (dto.PesoAlNacer <= 0 || dto.PesoAlNacer > 100)
                return BadRequest("El peso al nacer debe ser mayor a 0 y menor o igual a 100 kg.");

            if (string.IsNullOrWhiteSpace(dto.Raza))
                return BadRequest("La raza es obligatoria.");

            if (dto.FechaNacimiento.Date > DateTime.Today)
                return BadRequest("La fecha de nacimiento no puede ser posterior a hoy.");

            if (!Enum.IsDefined(dto.Sexo))
                return BadRequest("El sexo debe ser 'Macho' o 'Hembra'.");

            var madreInformada = CaravanaHelper.ProgenitorInformado(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre);
            if (madreInformada)
            {
                var error = CaravanaHelper.ErrorProgenitor(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre, "la madre");
                if (error is not null)
                    return BadRequest(error);
            }

            var padreInformado = CaravanaHelper.ProgenitorInformado(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre);
            if (padreInformado)
            {
                var error = CaravanaHelper.ErrorProgenitor(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre, "el padre");
                if (error is not null)
                {
                    return BadRequest(error);
                }
            }

            if (madreInformada &&
                CaravanaHelper.MismaCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre,
                                             dto.CaravanaCuig, dto.CaravanaNroManejo))
                return BadRequest("El animal no puede ser su propia madre");

            if (padreInformado &&
                CaravanaHelper.MismaCaravana(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre,
                                             dto.CaravanaCuig, dto.CaravanaNroManejo))
                return BadRequest("El animal no puede ser su propio padre");

            if (madreInformada && padreInformado &&
                CaravanaHelper.MismaCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre,
                                 dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre))
                return BadRequest("La madre y el padre no pueden ser el mismo animal.");

            Animal? madre = null;
            if (madreInformada)
            {
                madre = await _animalRepo.BuscarPorCaravana(dto.CaravanaCuigMadre!, dto.CaravanaNroManejoMadre!);
                if (madre == null)
                    return BadRequest("No se encontró un animal con la caravana de la madre indicada.");
            }

            Animal? padre = null;
            if (padreInformado)
            {
                padre = await _animalRepo.BuscarPorCaravana(dto.CaravanaCuigPadre!, dto.CaravanaNroManejoPadre!);
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
            if (!CaravanaHelper.ParteValida(cuig) || !CaravanaHelper.ParteValida(nroManejo))
                return BadRequest(CaravanaHelper.MensajeCaravanaInvalida);

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
            if (!CaravanaHelper.ParteValida(cuig) || !CaravanaHelper.ParteValida(nroManejo))
                return BadRequest(CaravanaHelper.MensajeCaravanaInvalida);

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