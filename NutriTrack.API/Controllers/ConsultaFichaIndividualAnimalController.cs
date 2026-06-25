using Microsoft.AspNetCore.Mvc;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;
using NutriTrack.API.DTOs;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

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
            //validar y ver que no este vacio
            //El campo es obligatorio
            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(nroManejo))
                return BadRequest("La caravana es obligatoria");
            //validar formato (6-8 caract y que esten los dos datos obligatorios juntos
            var caravanaCompleta = cuig + nroManejo;
            if (caravanaCompleta.Length < 6 || caravanaCompleta.Length > 10 || !caravanaCompleta.All(char.IsLetterOrDigit))
                return BadRequest("Formato de caravana invalido (alfanumerico, 6-8 caracteres)");

            var animal = await _repository.BuscarPorCaravana(cuig, nroManejo);
            if (animal == null)
                return NotFound("No se encontro un animal con esa caravana");

            var UltimoPeso = await _repository.BuscarPorUltimoPeso(animal.Id);

            var ficha_individual = new ConsultaFichaIndividualAnimalDTO
            {
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
                Padre = animal.Padre != null ? $"{animal.Padre.CaravanaCuig}--{animal.Padre.CaravanaNroManejo}" : null,
                UltimoPeso = UltimoPeso != null ? new UltimoPesoDTO
                {
                    FechaPesaje = UltimoPeso.FechaPesaje,
                    PesoKg = UltimoPeso.PesoKg

                } : null
            };

            return Ok(ficha_individual);

        }
    }
}
               