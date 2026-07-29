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
        private readonly EdicionFichaAnimalRepository animalRepository;

        public EdicionFichaAnimalController(EdicionFichaAnimalRepository animalRepository)
        {
            this.animalRepository = animalRepository;
        }

        [HttpPut]
        public async Task<IActionResult> Editar ([FromQuery] string cuig, [FromQuery] string NroManejo, [FromBody] EdicionFichaAnimalDTO dto)
        {
            //validar que haya caravana, que no este vacia
            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(NroManejo))
            {
                return BadRequest("La caravana es obligatoria");
            }
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
            if (dto.Sexo )
                return BadRequest("")

            






    }
    }
}
