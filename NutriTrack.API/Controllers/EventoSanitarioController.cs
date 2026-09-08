using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Entities;
using NutriTrack.Core.Entities.Enums;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoSanitarioController : ControllerBase
    {
        private readonly EventoSanitarioRepository _eventoRepo;
        private readonly AnimalRepository _animalRepo;
        private readonly MedicamentoRepository _medicamentoRepo;

        public EventoSanitarioController(
            EventoSanitarioRepository eventoRepo,
            AnimalRepository animalRepo,
            MedicamentoRepository medicamentoRepo)
        {
            _eventoRepo = eventoRepo;
            _animalRepo = animalRepo;
            _medicamentoRepo = medicamentoRepo;
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> RegistrarMultiple([FromBody] RegistrarEventoSanitarioMultipleDTO dto)
        {
            if (!Enum.TryParse<TipoEvento>(dto.TipoEvento, ignoreCase: true, out var tipoEvento))
                return BadRequest($"tipo_evento inválido. Opciones: {string.Join(", ", Enum.GetNames<TipoEvento>())}");

            if (dto.FechaEvento.Date > DateTime.Today)
                return BadRequest("fecha_evento no puede ser posterior a la fecha actual.");
 
            if (dto.VigenciaHasta.HasValue && dto.VigenciaHasta.Value.Date < dto.FechaEvento.Date)
                return BadRequest("vigencia hasta debe ser igual o posterior a fecha evento.");

            if (dto.FechaProximaAplicacion.HasValue && dto.FechaProximaAplicacion.Value.Date < dto.FechaEvento.Date)
                return BadRequest("fecha proxima aplicacion debe ser igual o posterior a fecha evento.");

            var detallesValidados = new List<DetalleMedicamento>();
            if (dto.DetallesMedicamento is not null)
            {
                foreach (var d in dto.DetallesMedicamento)
                {
                    var medicamento = await _medicamentoRepo.ObtenerPorIdAsync(d.IdMedicamento);
                    if (medicamento is null)
                        return BadRequest($"El medicamento con id {d.IdMedicamento} no existe en el catálogo.");

                    if (d.Dosis.HasValue && d.Dosis.Value <= 0)
                        return BadRequest("La dosis debe ser mayor a 0.");

                    if (d.Dosis.HasValue &&
                        (string.IsNullOrWhiteSpace(d.Unidad) || !Enum.TryParse<UnidadDosis>(d.Unidad, ignoreCase: true, out _)))
                        return BadRequest($"Debe informar una unidad válida ({string.Join(", ", Enum.GetNames<UnidadDosis>())}) si se informa dosis.");

                    detallesValidados.Add(new DetalleMedicamento
                    {
                        IdMedicamento = d.IdMedicamento,
                        Dosis = d.Dosis,
                        Unidad = d.Unidad,
                        Observaciones = d.Observaciones
                    });
                }
            }

            var animalesActivosDelRodeo = await _animalRepo.ObtenerActivosPorRodeo(dto.IdRodeo);
            if (animalesActivosDelRodeo.Count == 0)
                return BadRequest("No se encontró el rodeo seleccionado o no tiene animales activos.");

            List<Animal> animalesSeleccionados;

            if (dto.ModoSeleccion == "Rodeo completo")
            {
                animalesSeleccionados = animalesActivosDelRodeo;
            }
            else if (dto.ModoSeleccion == "Selección manual")
            {
                if (dto.Caravanas is null || dto.Caravanas.Count == 0)
                    return BadRequest("Debe seleccionar al menos un animal.");

                var duplicadas = dto.Caravanas.GroupBy(c => c).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
                if (duplicadas.Any())
                    return BadRequest($"Caravanas repetidas en la selección: {string.Join(", ", duplicadas)}");

                animalesSeleccionados = new List<Animal>();
                var errores = new List<string>();

                foreach (var caravana in dto.Caravanas)
                {
                    var animal = animalesActivosDelRodeo.FirstOrDefault(a =>
                        a.CaravanaCuig == caravana || a.CaravanaNroManejo == caravana);

                    if (animal is null)
                        errores.Add($"'{caravana}' no existe, no está activa o no pertenece al rodeo seleccionado.");
                    else
                        animalesSeleccionados.Add(animal);
                }

                if (errores.Any())
                    return BadRequest(string.Join(" ", errores));
            }
            else
            {
                return BadRequest("modo_seleccion inválido. Opciones: 'Rodeo completo' o 'Selección manual'.");
            }

            int idUsuarioLogueado = 1; // TODO: reemplazar cuando se resuelva el issue de Usuario

            foreach (var animal in animalesSeleccionados)
            {
                var evento = new EventoSanitario
                {
                    TipoEvento = tipoEvento.ToString(),
                    FechaEvento = dto.FechaEvento,
                    VigenciaHasta = dto.VigenciaHasta,
                    FechaProximaAplicacion = dto.FechaProximaAplicacion,
                    Observaciones = dto.Observaciones,
                    IdUsuario = idUsuarioLogueado,
                    IdAnimal = animal.Id
                };

                foreach (var detalle in detallesValidados)
                {
                    evento.DetallesMedicamento.Add(new DetalleMedicamento
                    {
                        IdMedicamento = detalle.IdMedicamento,
                        Dosis = detalle.Dosis,
                        Unidad = detalle.Unidad,
                        Observaciones = detalle.Observaciones
                    });
                }

                await _eventoRepo.AgregarAsync(evento);
            }

            return Ok(new EventoSanitarioResponseDTO
            {
                CantidadAnimalesAlcanzados = animalesSeleccionados.Count,
                TipoEvento = tipoEvento.ToString(),
                FechaEvento = dto.FechaEvento
            });
        }
    }
}