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
    [Authorize(Roles = $"{RolesUsuario.Administrador},{RolesUsuario.EncargadoDeCampo},{RolesUsuario.AsesorTecnico}")]
    public class ConsultaFichaIndividualAnimalController : ControllerBase
    {
        private readonly ConsultaFichaIndividualAnimalRepository _repository;
        public ConsultaFichaIndividualAnimalController(ConsultaFichaIndividualAnimalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Consultar([FromQuery] string cuig, [FromQuery] string nroManejo)
        {
            if (!CaravanaHelper.ParteValida(cuig) || !CaravanaHelper.ParteValida(nroManejo))
                return BadRequest(CaravanaHelper.MensajeCaravanaInvalida);

            var animal = await _repository.BuscarPorCaravana(cuig, nroManejo);
            if (animal == null)
                return NotFound("No se encontro un animal con esa caravana");

            var ultimoPeso = await _repository.BuscarPorUltimoPeso(animal.Id);

            var fichaIndividual = new ConsultaFichaIndividualAnimalDTO
            {
                Id = animal.Id,
                CaravanaCuig = animal.CaravanaCuig,
                CaravanaNroManejo = animal.CaravanaNroManejo,
                FechaNacimiento = animal.FechaNacimiento,
                PesoAlNacer = animal.PesoAlNacer,
                Sexo = animal.Sexo.ToString(),
                Raza = animal.Raza,
                FechaAlta = animal.FechaAlta,
                ColorPelaje = animal.ColorPelaje,
                Estado = animal.Estado ? "Activo" : "Inactivo",
                RodeoActual = animal.Rodeo?.Nombre,
                Madre = animal.Madre != null ? $"{animal.Madre.CaravanaCuig}-{animal.Madre.CaravanaNroManejo}" : null,
                Padre = animal.Padre != null ? $"{animal.Padre.CaravanaCuig}-{animal.Padre.CaravanaNroManejo}" : null,
                UltimoPeso = ultimoPeso != null ? new UltimoPesoDTO
                {
                    Id = ultimoPeso.Id,
                    FechaPesaje = ultimoPeso.FechaPesaje,
                    PesoKg = ultimoPeso.PesoKg,
                    Observaciones = ultimoPeso.Observaciones
                } : null
            };

            return Ok(fichaIndividual);
        }
    }
}
               