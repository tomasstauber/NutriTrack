using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.API.GeneracionReportesPdf;
using NutriTrack.Core.Reportes;
using NutriTrack.Infraestructure.Repositories;
using System;
using System.Linq;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RolesUsuario.Administrador)]
    public class ReporteInventarioAnimalesController : ControllerBase
    {
        private readonly ReporteInventarioAnimalesRepository _repository;
        private readonly IReportePdfService _pdfService;

        public ReporteInventarioAnimalesController(
            ReporteInventarioAnimalesRepository repository,
            IReportePdfService pdfService)
        {
            _repository = repository;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> Generar([FromQuery] ReporteInventarioAnimalesDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            return error ?? Ok(reporte);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarPdf([FromQuery] ReporteInventarioAnimalesDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            if (error != null)
                return error;

            var bytes = _pdfService.GenerarInventario(reporte!);
            var nombreArchivo = $"inventario_animales_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }

        private async Task<(IActionResult? Error, ReporteInventarioAnimalesResponseDTO? Reporte)> ArmarReporteAsync(
            ReporteInventarioAnimalesDTO filtro)
        {
        
            if (filtro.IdRodeo.HasValue && !await _repository.ExisteRodeo(filtro.IdRodeo.Value))
                return (NotFound("No se encontró el rodeo seleccionado"), null);

            if (string.IsNullOrEmpty(filtro.TipoReporte) ||
                !PeriodoReporteCalculator.TiposValidos.Contains(filtro.TipoReporte.ToLower()))
                return (BadRequest("El tipo de reporte ingresado no es válido"), null);

            var tipo = filtro.TipoReporte.ToLower();
            DateTime desde;
            DateTime hasta;

            if (tipo == "personalizado")
            {
     
                if (!filtro.Desde.HasValue || !filtro.Hasta.HasValue)
                    return (BadRequest("Debe indicar desde y hasta para un reporte personalizado"), null);

                if (filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                    return (BadRequest("El período ingresado no es válido"), null);

                desde = filtro.Desde.Value;
                hasta = filtro.Hasta.Value;
            }
            else
            {
               
                if (tipo != "actual" && !filtro.FechaReferencia.HasValue)
                    return (BadRequest("Debe indicar la fecha del período para este tipo de reporte"), null);

                var referencia = filtro.FechaReferencia ?? DateTime.Today;
                (desde, hasta) = PeriodoReporteCalculator.Calcular(tipo, referencia);
            }

            var animales = await _repository.ObtenerInventario(filtro.IdRodeo, desde, hasta);

            if (!animales.Any())
                return (NotFound("No se encontraron animales para el alcance y período seleccionados."), null);

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

            var reporte = new ReporteInventarioAnimalesResponseDTO
            {
                TotalAnimales = items.Count,
                Animales = items
            };

            return (null, reporte);
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