using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EdicionFichaAnimalController : ControllerBase
    {
        private readonly EdicionFichaAnimalRepository _repository;

        public EdicionFichaAnimalController(EdicionFichaAnimalRepository repository)
        {
            _repository = repository;
        }

        [HttpPut]
        public async Task<IActionResult> Editar ([FromQuery] string cuig, [FromQuery] string NroManejo, [FromBody] EdicionFichaAnimalDTO dto)
        {
            //validar que haya caravana, que no este vacia
            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(NroManejo))
            {
                return BadRequest("La caravana es obligatoria");
            }
            
            // Validar formato alfanumérico 6-8 caracteres
            var caravanaCompleta = cuig + NroManejo;
            if (caravanaCompleta.Length < 6 || caravanaCompleta.Length > 10 ||
                !caravanaCompleta.All(char.IsLetterOrDigit))
                return BadRequest("Formato de caravana inválido (alfanumérico, 6-8 caracteres).");
            
            // Buscar el recurso a editar (¿existe el animal?)
            //buscar animal
            var animal = await _repository.BuscarPorCaravana(cuig, NroManejo);
            if (animal == null)
                return NotFound("No se encontro un animal con esa caravana");
            //validar peso
            if (dto.PesoAlNacer <= 0 || dto.PesoAlNacer > 100)
                return BadRequest("El peso al nacer debe ser mayor a 0 y menor o igual a 100kg");
            //validar fecha nacimiento
            if (dto.FechaNacimiento > DateTime.Now)
                return BadRequest("La fecha de nacimiento no puede ser posterior a hoy");
            //validar sexo
            if (!Enum.TryParse<Sexo>(dto.Sexo, ignoreCase: true, out var sexo))
                return BadRequest("El sexo debe ser 'Macho' o 'Hembra'.");
            // Validar y resolver madre
            Animal? madre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigMadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoMadre))
            {
                madre = await _repository.BuscarPorCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre);
                if (madre == null)
                    return BadRequest("No se encontró un animal con la caravana de la madre indicada.");
                if (madre.Id == animal.Id)
                    return BadRequest("El animal no puede ser su propia madre.");
            }

            // Validar y resolver padre
            Animal? padre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigPadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoPadre))
            {
                padre = await _repository.BuscarPorCaravana(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre);
                if (padre == null)
                    return BadRequest("No se encontró un animal con la caravana del padre indicada.");
                if (padre.Id == animal.Id)
                    return BadRequest("El animal no puede ser su propio padre.");
            }

            // Actualizar campos editables
            animal.FechaNacimiento = dto.FechaNacimiento;
            animal.PesoAlNacer = dto.PesoAlNacer;
            animal.Sexo = sexo;
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
