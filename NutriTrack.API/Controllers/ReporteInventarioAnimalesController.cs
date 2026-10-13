using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.API.GeneracionReportesPdf;
using NutriTrack.Core.Entities;
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
            var (error, reporte, _) = await ArmarReporteAsync(filtro);
            return error ?? Ok(reporte);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarPdf([FromQuery] ReporteInventarioAnimalesDTO filtro)
        {
            var (error, reporte, encabezado) = await ArmarReporteAsync(filtro);
            if (error != null)
                return error;

            // Una sola fecha para el encabezado y el nombre del archivo
            var generado = DateTime.Now;
            encabezado!.Generado = generado;

            var bytes = _pdfService.GenerarInventario(reporte!, encabezado);
            var nombreArchivo = $"inventario_animales_{generado:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }

        // El encabezado solo lo usa el PDF: el JSON no cambia
        private async Task<(IActionResult? Error, ReporteInventarioAnimalesResponseDTO? Reporte, EncabezadoReporte? Encabezado)> ArmarReporteAsync(
            ReporteInventarioAnimalesDTO filtro)
        {
        
            // Una sola consulta: valida el rodeo y trae el nombre para el encabezado
            string? nombreRodeo = null;
            if (filtro.IdRodeo.HasValue)
            {
                nombreRodeo = await _repository.ObtenerNombreRodeo(filtro.IdRodeo.Value);
                if (nombreRodeo == null)
                    return (NotFound("No se encontró el rodeo seleccionado"), null, null);
            }

            if (string.IsNullOrEmpty(filtro.TipoReporte) ||
                !PeriodoReporteCalculator.TiposValidos.Contains(filtro.TipoReporte.ToLower()))
                return (BadRequest("El tipo de reporte ingresado no es válido"), null, null);

            var tipo = filtro.TipoReporte.ToLower();
            List<Animal> animales;

            // Quedan en null en el inventario completo: el encabezado dice "Stock activo al"
            DateTime? desdeEncabezado = null;
            DateTime? hastaEncabezado = null;

            if (tipo == "actual")
            {
                // Inventario completo: todos los activos, sin período.
                // FechaReferencia, Desde y Hasta se ignoran si llegan
                animales = await _repository.ObtenerInventario(filtro.IdRodeo);
            }
            else if (tipo == "personalizado")
            {
     
                if (!filtro.Desde.HasValue || !filtro.Hasta.HasValue)
                    return (BadRequest("Debe indicar desde y hasta para un reporte personalizado"), null, null);

                if (filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                    return (BadRequest("El período ingresado no es válido"), null, null);

                animales = await _repository.ObtenerInventario(
                    filtro.IdRodeo, filtro.Desde.Value, filtro.Hasta.Value);
                desdeEncabezado = filtro.Desde.Value;
                hastaEncabezado = filtro.Hasta.Value;
            }
            else
            {
               
                if (!filtro.FechaReferencia.HasValue)
                    return (BadRequest("Debe indicar la fecha del período para este tipo de reporte"), null, null);

                var (desde, hasta) = PeriodoReporteCalculator.Calcular(tipo, filtro.FechaReferencia.Value);
                animales = await _repository.ObtenerInventario(filtro.IdRodeo, desde, hasta);
                desdeEncabezado = desde;
                hastaEncabezado = hasta;
            }

            if (!animales.Any())
                return (NotFound("No se encontraron animales para el alcance y período seleccionados."), null, null);

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

            var encabezado = new EncabezadoReporte
            {
                Desde = desdeEncabezado,
                Hasta = hastaEncabezado,
                NombreRodeo = nombreRodeo
            };

            return (null, reporte, encabezado);
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