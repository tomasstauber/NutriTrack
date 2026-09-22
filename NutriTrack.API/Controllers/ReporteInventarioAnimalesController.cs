using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.DTOs;
using NutriTrack.Core.Reportes;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteInventarioAnimalesController : ControllerBase
    {
        private readonly ReporteInventarioAnimalesRepository _repository;

        public ReporteInventarioAnimalesController(ReporteInventarioAnimalesRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Generar([FromQuery] ReporteInventarioAnimalesDTO filtro)
        {
            if (string.IsNullOrEmpty(filtro.TipoReporte) ||
                !PeriodoReporteCalculator.TiposValidos.Contains(filtro.TipoReporte.ToLower()))
                return BadRequest("El tipo de reporte ingresado no es válido");

            if (filtro.IdRodeo.HasValue && !await _repository.ExisteRodeo(filtro.IdRodeo.Value))
                return NotFound("No se encontró el rodeo seleccionado");

            var tipo = filtro.TipoReporte.ToLower();
            DateTime? desde = filtro.Desde;
            DateTime? hasta = filtro.Hasta;

            if (tipo == "personalizado")
            {
                if (!desde.HasValue || !hasta.HasValue)
                    return BadRequest("Debe indicar desde y hasta para un reporte personalizado");

                if (desde.Value.Date > hasta.Value.Date)
                    return BadRequest("El período ingresado no es válido");
            }
            else
            {
                if (tipo != "actual" && !filtro.FechaReferencia.HasValue)
                    return BadRequest("Debe indicar la fecha del período para este tipo de reporte");

                var referencia = filtro.FechaReferencia ?? DateTime.Today;
                (desde, hasta) = PeriodoReporteCalculator.Calcular(tipo, referencia);
            }

            var animales = await _repository.ObtenerInventario(filtro.IdRodeo, desde.Value, hasta.Value);

            
            if (!animales.Any())
                return NotFound("No se encontraron animales para el alcance y período seleccionados.");

            var hoy = DateTime.Today;

            var items = animales.Select(a => new AnimalInventarioItemDTO
            {
                Caravana = $"{a.CaravanaCuig}-{a.CaravanaNroManejo}",
                Raza = a.Raza,
                Sexo = a.Sexo.ToString(),
                Edad = CalcularEdadTexto(a.FechaNacimiento, hoy), 
                FechaAltaSistema = a.FechaAlta,
                EstadoActual = a.Estado ? "Activo" : "Inactivo",
                RodeoActual = a.Rodeo?.Nombre
            }).ToList();

            return Ok(new ReporteInventarioAnimalesResponseDTO
            {
                TotalAnimales = items.Count,
                Animales = items
            });
        }
        private static string CalcularEdadTexto(DateTime fechaNacimiento, DateTime hoy)
        {
            var totalMeses = ((hoy.Year - fechaNacimiento.Year) * 12) + hoy.Month - fechaNacimiento.Month;
            if (hoy.Day < fechaNacimiento.Day) totalMeses--;

            var anios = totalMeses / 12;
            var meses = totalMeses % 12;

            return anios > 0
                ? $"{anios} año{(anios != 1 ? "s" : "")} y {meses} mes{(meses != 1 ? "es" : "")}"
                : $"{meses} mes{(meses != 1 ? "es" : "")}";
        }
    }
}