using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriTrack.API.Constants;
using NutriTrack.API.DTOs;
using NutriTrack.Infraestructure.Repositories;
using System;
using System.Linq;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RolesUsuario.Administrador)]
    public class AlertaController : ControllerBase
    {
        // Rango por defecto si Alertas:DiasAviso no está o no es válido 
        private const int DiasAvisoPorDefecto = 7;

        private readonly ReporteFechasImportantesRepository _fechasImportantesRepository;
        private readonly AlertaPlanAlimenticioRepository _planRepository;
        private readonly IConfiguration _config;

        public AlertaController(
            ReporteFechasImportantesRepository fechasImportantesRepository,
            AlertaPlanAlimenticioRepository planRepository,
            IConfiguration config)
        {
            _fechasImportantesRepository = fechasImportantesRepository;
            _planRepository = planRepository;
            _config = config;
        }

        // Alertas vigentes, calculadas al consultar. Sin alertas: 200 con lista vacía
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var diasAviso = ObtenerDiasAviso();
            var desde = DateTime.Today;
            var hasta = desde.AddDays(diasAviso);
            var hoy = DateOnly.FromDateTime(desde);

            // Sanitarias: mismas consultas que el reporte de fechas importantes, sin filtro de rodeo
            var proximas = await _fechasImportantesRepository.ObtenerProximasAplicaciones(null, desde, hasta);
            var vencimientos = await _fechasImportantesRepository.ObtenerVencimientos(null, desde, hasta);
            var planes = await _planRepository.ObtenerVencimientos(hoy, DateOnly.FromDateTime(hasta));

            var alertas = proximas
                .Select(p => ArmarSanitaria("ProximaAplicacion", p, p.FechaProximaAplicacion, hoy))
                .Concat(vencimientos.Select(v => ArmarSanitaria("Vencimiento", v, v.VigenciaHasta, hoy)))
                .Concat(planes.Select(p => new AlertaItemDTO
                {
                    Tipo = "PlanAlimenticio",
                    Subtipo = "Vencimiento",
                    Destino = p.NombreRodeo,
                    Descripcion = p.NombrePlan,
                    Fecha = p.VigenciaHasta,
                    DiasRestantes = p.VigenciaHasta.DayNumber - hoy.DayNumber,
                    VigenciaDesde = p.VigenciaDesde,
                    CantidadAnimales = p.CantidadAnimales
                }))
                // Orden fijo: a igual fecha, el resultado no cambia entre llamadas
                .OrderBy(a => a.Fecha)
                .ThenBy(a => a.Tipo, StringComparer.Ordinal)
                .ThenBy(a => a.Destino, StringComparer.Ordinal)
                .ThenBy(a => a.Subtipo, StringComparer.Ordinal)
                .ThenBy(a => a.Descripcion, StringComparer.Ordinal)
                .ToList();

            return Ok(new AlertaResponseDTO
            {
                DiasAviso = diasAviso,
                Total = alertas.Count,
                Alertas = alertas
            });
        }

        // Descripción: los medicamentos del evento o, si no tiene, el tipo de evento
        private static AlertaItemDTO ArmarSanitaria(string subtipo, ReporteEventoRawBase evento, DateTime fecha, DateOnly hoy)
        {
            var dia = DateOnly.FromDateTime(fecha);
            return new AlertaItemDTO
            {
                Tipo = "Sanitaria",
                Subtipo = subtipo,
                Destino = evento.Destino,
                Descripcion = string.IsNullOrEmpty(evento.Producto) ? evento.TipoEvento : evento.Producto,
                Fecha = dia,
                DiasRestantes = dia.DayNumber - hoy.DayNumber
            };
        }

        // La clave es opcional: sin ella, o con un valor no válido, se usan 7 días
        private int ObtenerDiasAviso()
        {
            return int.TryParse(_config["Alertas:DiasAviso"], out var dias) && dias > 0
                ? dias
                : DiasAvisoPorDefecto;
        }
    }
}
