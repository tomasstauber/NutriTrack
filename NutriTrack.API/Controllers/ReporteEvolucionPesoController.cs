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
    public class ReporteEvolucionPesoController : ControllerBase
    {
        private readonly ReporteEvolucionPesoRepository _repository;
        private readonly IReportePdfService _pdfService;

        public ReporteEvolucionPesoController(
            ReporteEvolucionPesoRepository repository,
            IReportePdfService pdfService)
        {
            _repository = repository;
            _pdfService = pdfService;
        }

        [HttpGet]
        public async Task<IActionResult> Generar([FromQuery] ReporteEvolucionPesoDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            return error ?? Ok(reporte);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarPdf([FromQuery] ReporteEvolucionPesoDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            if (error != null)
                return error;

            var bytes = _pdfService.GenerarEvolucionPeso(reporte!);
            var nombreArchivo = $"evolucion_peso_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }

        private async Task<(IActionResult? Error, ReporteEvolucionPesoResponseDTO? Reporte)> ArmarReporteAsync(
            ReporteEvolucionPesoDTO filtro)
        {
            var (error, desde, hasta) = await ValidarYCalcularPeriodoAsync(filtro);
            if (error != null) return (error, null);

            int? idAnimalFiltro = null;
            if (!string.IsNullOrWhiteSpace(filtro.Caravana))
            {
                var animal = await _repository.BuscarAnimalPorCaravana(filtro.Caravana);
                if (animal == null)
                    return (NotFound("No se encontró un animal con esa caravana"), null);
                idAnimalFiltro = animal.Id;
            }

            var filas = await _repository.ObtenerResumenPorAnimal(filtro.IdRodeo, desde, hasta, idAnimalFiltro);

            if (filas.Count == 0)
                return (NotFound("No se encontraron registros de peso para el período seleccionado."), null);

            var reporte = new ReporteEvolucionPesoResponseDTO
            {
                Animales = filas.Select(f => new AnimalEvolucionPesoItemDTO
                {
                    Caravana = f.Caravana,
                    EstadoActual = f.Estado ? "Activo" : "Inactivo",
                    FechaInicial = f.FechaInicial,
                    PesoInicial = f.PesoInicial,
                    FechaFinal = f.FechaFinal,
                    PesoFinal = f.PesoFinal,
                    VariacionKg = f.VariacionKg,
                    CantidadRegistros = f.CantidadRegistros
                }).ToList()
            };

            if (idAnimalFiltro.HasValue)
            {
                var pesajes = await _repository.ObtenerDetallePesajes(idAnimalFiltro.Value, desde, hasta);
                reporte.Detalle = new DetallePesajesDTO
                {
                    Pesajes = pesajes.Select(p => new PesajeDetalleItemDTO
                    {
                        FechaPesaje = p.FechaPesaje,
                        PesoKg = p.PesoKg
                    }).ToList()
                };
            }

            return (null, reporte);
        }

        private async Task<(IActionResult? Error, DateTime Desde, DateTime Hasta)> ValidarYCalcularPeriodoAsync(
            ReporteEvolucionPesoDTO filtro)
        {
            if (filtro.IdRodeo.HasValue && !await _repository.ExisteRodeo(filtro.IdRodeo.Value))
                return (NotFound("No se encontró el rodeo seleccionado"), default, default);

            if (string.IsNullOrEmpty(filtro.TipoReporte) ||
                !PeriodoReporteCalculator.TiposValidos.Contains(filtro.TipoReporte.ToLower()))
                return (BadRequest("El tipo de reporte ingresado no es válido"), default, default);

            var tipo = filtro.TipoReporte.ToLower();

            if (tipo == "personalizado")
            {
                if (!filtro.Desde.HasValue || !filtro.Hasta.HasValue)
                    return (BadRequest("Debe indicar desde y hasta para un reporte personalizado"), default, default);

                if (filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                    return (BadRequest("El período ingresado no es válido"), default, default);

                return (null, filtro.Desde.Value, filtro.Hasta.Value);
            }

            if (tipo != "actual" && !filtro.FechaReferencia.HasValue)
                return (BadRequest("Debe indicar la fecha del período para este tipo de reporte"), default, default);

            var referencia = filtro.FechaReferencia ?? DateTime.Today;
            var (desde, hasta) = PeriodoReporteCalculator.Calcular(tipo, referencia);
            return (null, desde, hasta);
        }
    }
}