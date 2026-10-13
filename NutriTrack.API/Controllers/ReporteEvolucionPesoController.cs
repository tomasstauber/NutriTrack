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
            var (error, reporte, _) = await ArmarReporteAsync(filtro);
            return error ?? Ok(reporte);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarPdf([FromQuery] ReporteEvolucionPesoDTO filtro)
        {
            var (error, reporte, encabezado) = await ArmarReporteAsync(filtro);
            if (error != null)
                return error;

            // Una sola fecha para el encabezado y el nombre del archivo
            var generado = DateTime.Now;
            encabezado!.Generado = generado;

            var bytes = _pdfService.GenerarEvolucionPeso(reporte!, encabezado);
            var nombreArchivo = $"evolucion_peso_{generado:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }

        // El encabezado solo lo usa el PDF: el JSON no cambia
        private async Task<(IActionResult? Error, ReporteEvolucionPesoResponseDTO? Reporte, EncabezadoReporte? Encabezado)> ArmarReporteAsync(
            ReporteEvolucionPesoDTO filtro)
        {
            var (error, desde, hasta, nombreRodeo) = await ValidarYCalcularPeriodoAsync(filtro);
            if (error != null) return (error, null, null);

            int? idAnimalFiltro = null;
            string? caravana = null;
            if (!string.IsNullOrWhiteSpace(filtro.Caravana))
            {
                var animal = await _repository.BuscarAnimalPorCaravana(filtro.Caravana);
                if (animal == null)
                    return (NotFound("No se encontró un animal con esa caravana"), null, null);
                idAnimalFiltro = animal.Id;

                // La del animal encontrado, con el mismo formato que la tabla
                caravana = $"{animal.CaravanaCuig}-{animal.CaravanaNroManejo}";
            }

            var filas = await _repository.ObtenerResumenPorAnimal(filtro.IdRodeo, desde, hasta, idAnimalFiltro);

            if (filas.Count == 0)
                return (NotFound("No se encontraron registros de peso para el período seleccionado."), null, null);

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

            var encabezado = new EncabezadoReporte
            {
                Desde = desde,
                Hasta = hasta,
                NombreRodeo = nombreRodeo,
                Caravana = caravana
            };

            return (null, reporte, encabezado);
        }

        // Devuelve también el nombre del rodeo (null sin filtro) para el encabezado del PDF
        private async Task<(IActionResult? Error, DateTime Desde, DateTime Hasta, string? NombreRodeo)> ValidarYCalcularPeriodoAsync(
            ReporteEvolucionPesoDTO filtro)
        {
            // Una sola consulta: valida el rodeo y trae el nombre
            string? nombreRodeo = null;
            if (filtro.IdRodeo.HasValue)
            {
                nombreRodeo = await _repository.ObtenerNombreRodeo(filtro.IdRodeo.Value);
                if (nombreRodeo == null)
                    return (NotFound("No se encontró el rodeo seleccionado"), default, default, null);
            }

            if (string.IsNullOrEmpty(filtro.TipoReporte) ||
                !PeriodoReporteCalculator.TiposValidos.Contains(filtro.TipoReporte.ToLower()))
                return (BadRequest("El tipo de reporte ingresado no es válido"), default, default, null);

            var tipo = filtro.TipoReporte.ToLower();

            if (tipo == "personalizado")
            {
                if (!filtro.Desde.HasValue || !filtro.Hasta.HasValue)
                    return (BadRequest("Debe indicar desde y hasta para un reporte personalizado"), default, default, null);

                if (filtro.Desde.Value.Date > filtro.Hasta.Value.Date)
                    return (BadRequest("El período ingresado no es válido"), default, default, null);

                return (null, filtro.Desde.Value, filtro.Hasta.Value, nombreRodeo);
            }

            if (tipo != "actual" && !filtro.FechaReferencia.HasValue)
                return (BadRequest("Debe indicar la fecha del período para este tipo de reporte"), default, default, null);

            var referencia = filtro.FechaReferencia ?? DateTime.Today;
            var (desde, hasta) = PeriodoReporteCalculator.Calcular(tipo, referencia);
            return (null, desde, hasta, nombreRodeo);
        }
    }
}