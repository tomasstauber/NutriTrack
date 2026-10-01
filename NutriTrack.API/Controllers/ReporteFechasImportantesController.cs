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
    public class ReporteFechasImportantesController : ControllerBase
    {
        private readonly ReporteFechasImportantesRepository _repository;
        private readonly IReportePdfService _pdfService;

        public ReporteFechasImportantesController(
            ReporteFechasImportantesRepository repository,
            IReportePdfService pdfService)
        {
            _repository = repository;
            _pdfService = pdfService;
        }
        [HttpGet]
        public async Task<IActionResult> Generar([FromQuery] ReporteFechasImportantesDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            return error ?? Ok(reporte);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GenerarPdf([FromQuery] ReporteFechasImportantesDTO filtro)
        {
            var (error, reporte) = await ArmarReporteAsync(filtro);
            if (error != null)
                return error;

            var bytes = _pdfService.GenerarFechasImportantes(reporte!);
            var nombreArchivo = $"fechas_importantes_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(bytes, "application/pdf", nombreArchivo);
        }

        private async Task<(IActionResult? Error, ReporteFechasImportantesResponseDTO? Reporte)> ArmarReporteAsync(
            ReporteFechasImportantesDTO filtro)
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

            var proximas = await _repository.ObtenerProximasAplicaciones(filtro.IdRodeo, desde, hasta);
            var vencimientos = await _repository.ObtenerVencimientos(filtro.IdRodeo, desde, hasta);

            if (proximas.Count == 0 && vencimientos.Count == 0)
                return (NotFound("No hay fechas programadas para el período seleccionado."), null);

            var reporte = new ReporteFechasImportantesResponseDTO
            {
                ProximasAplicaciones = proximas.Select(p => new ProximaAplicacionItemDTO
                {
                    Destino = p.Destino,
                    TipoEvento = p.TipoEvento,
                    Producto = p.Producto,
                    FechaProximaAplicacion = p.FechaProximaAplicacion,
                    Responsable = p.Responsable
                }).ToList(),

                VencimientosEventos = vencimientos.Select(v => new VencimientoEventoItemDTO
                {
                    Destino = v.Destino,
                    TipoEvento = v.TipoEvento,
                    Producto = v.Producto,
                    VigenciaHasta = v.VigenciaHasta,
                    Responsable = v.Responsable,
                    Observaciones = v.Observaciones
                }).ToList()
            };

            return (null, reporte);
        }
    }
}