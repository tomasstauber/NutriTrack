using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalController : ControllerBase
    {
        private readonly AltaAnimalRepository _animalRepo;

        public AnimalController(AltaAnimalRepository animalRepo)
        {
            _animalRepo = animalRepo;
        }

        [HttpPost]
        public IActionResult Crear([FromBody] CrearAnimalDTO dto)
        {
            
            if (_animalRepo.ExisteCaravana(dto.CaravanaCuig, dto.CaravanaNroManejo))
                return Conflict("Ya existe un animal con esa caravana.");

            
            if (dto.PesoAlNacer <= 0 || dto.PesoAlNacer > 100)
                return BadRequest("El peso al nacer debe ser mayor a 0 y menor o igual a 100 kg.");

            
            if (dto.FechaNacimiento > DateTime.Now)
                return BadRequest("La fecha de nacimiento no puede ser posterior a hoy.");

            
            Animal? madre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigMadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoMadre))
            {
                madre = _animalRepo.BuscarPorCaravana(dto.CaravanaCuigMadre, dto.CaravanaNroManejoMadre);
                if (madre == null)
                    return BadRequest("No se encontró un animal con la caravana de la madre indicada.");
            }

            
            Animal? padre = null;
            if (!string.IsNullOrEmpty(dto.CaravanaCuigPadre) && !string.IsNullOrEmpty(dto.CaravanaNroManejoPadre))
            {
                padre = _animalRepo.BuscarPorCaravana(dto.CaravanaCuigPadre, dto.CaravanaNroManejoPadre);
                if (padre == null)
                    return BadRequest("No se encontró un animal con la caravana del padre indicada.");
            }

            // Convertir sexo de string a enum
            if (!Enum.TryParse<Sexo>(dto.Sexo, ignoreCase: true, out var sexo))
                return BadRequest("El sexo debe ser 'Macho' o 'Hembra'.");

           
            var animal = new Animal
            {
                CaravanaCuig = dto.CaravanaCuig,
                CaravanaNroManejo = dto.CaravanaNroManejo,
                FechaNacimiento = dto.FechaNacimiento,
                PesoAlNacer = dto.PesoAlNacer,
                MadreId = madre?.Id,
                PadreId = padre?.Id,
                Raza = dto.Raza,
                Sexo = sexo,
                ColorPelaje = dto.ColorPelaje,
                FechaAlta = DateTime.Now, // Eso lo asigna automaticamente el sistema
                Estado = true,            // activo por defecto
                RodeoId = null            // sin rodeo al dar de alta
            };

            _animalRepo.Crear(animal);

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
    }
}