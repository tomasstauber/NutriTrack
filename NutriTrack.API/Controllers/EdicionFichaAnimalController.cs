using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.API.Helpers;
using NutriTrack.Core.Entities;
using NutriTrack.Core.Entities.Enums;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo}")]
    public class EdicionFichaAnimalController : ControllerBase
    {
        private readonly EdicionFichaAnimalRepository _repository;

        public EdicionFichaAnimalController(EdicionFichaAnimalRepository repository)
        {
            _repository = repository;
        }

        [HttpPut]
        public async Task<IActionResult> Editar ([FromQuery] string cuig, [FromQuery] string nroManejo, [FromBody] EdicionFichaAnimalDTO dto)
        {
            if (!CaravanaHelper.ParteValida(cuig) || !CaravanaHelper.ParteValida(nroManejo))
                return BadRequest(CaravanaHelper.MensajeCaravanaInvalida);

            // Buscar el recurso a editar (¿existe el animal?)
            //buscar animal
            var animal = await _repository.BuscarPorCaravana(cuig, nroManejo);
            if (animal == null)
                return NotFound("No se encontro un animal con esa caravana");
            //validar peso
            if (dto.PesoAlNacer <= 0 || dto.PesoAlNacer > 100)
                return BadRequest("El peso al nacer debe ser mayor a 0 y menor o igual a 100kg");

            if (string.IsNullOrWhiteSpace(dto.Raza))
                return BadRequest("La raza es obligatoria.");

            //validar fecha nacimiento
            if (dto.FechaNacimiento.Date > DateTime.Today)
                return BadRequest("La fecha de nacimiento no puede ser posterior a hoy");

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
                    return BadRequest(error);
            }

            if (madreInformada && padreInformado
                && CaravanaHelper.MismaCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre,
                                                dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre))
                return BadRequest("La madre y el padre no pueden ser el mismo animal");

            // Validar y resolver madre
            Animal? madre = null;
            if (madreInformada)
            {
                madre = await _repository.BuscarPorCaravana(dto.CaravanaCuigMadre!, dto.CaravanaNroManejoMadre!);
                if (madre == null)
                    return BadRequest("No se encontró un animal con la caravana de la madre indicada.");
                if (madre.Id == animal.Id)
                    return BadRequest("El animal no puede ser su propia madre.");
            }

            // Validar y resolver padre
            Animal? padre = null;
            if (padreInformado)
            {
                padre = await _repository.BuscarPorCaravana(dto.CaravanaCuigPadre!, dto.CaravanaNroManejoPadre!);
                if (padre == null)
                    return BadRequest("No se encontró un animal con la caravana del padre indicada.");
                if (padre.Id == animal.Id)
                    return BadRequest("El animal no puede ser su propio padre.");
            }

            // Actualizar campos editables
            animal.FechaNacimiento = dto.FechaNacimiento;
            animal.PesoAlNacer = dto.PesoAlNacer;
            animal.Sexo = dto.Sexo;
            animal.Raza = dto.Raza;
            animal.ColorPelaje = dto.ColorPelaje;
            animal.MadreId = madre?.Id;
            animal.PadreId = padre?.Id;

            await _repository.Actualizar(animal);

            return Ok(new
            {
                animal.Id,
                animal.CaravanaCuig,
                animal.CaravanaNroManejo,
                animal.FechaNacimiento,
                animal.PesoAlNacer,
                Sexo = animal.Sexo.ToString(),
                animal.Raza,
                animal.ColorPelaje,
                animal.FechaAlta,
                animal.Estado,
                Madre = madre != null ? $"{madre.CaravanaCuig}-{madre.CaravanaNroManejo}" : null,
                Padre = padre != null ? $"{padre.CaravanaCuig}-{padre.CaravanaNroManejo}" : null
            });
        }
    }
}
