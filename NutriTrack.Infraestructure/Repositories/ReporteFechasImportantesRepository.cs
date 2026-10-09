using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    
    public class ReporteFechasImportantesRepository
    {
        private readonly AppDbContext _context;

        public ReporteFechasImportantesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteRodeo(int idRodeo)
        {
            return await _context.Rodeos.AnyAsync(r => r.Id == idRodeo && r.Activo);
        }

        public async Task<List<ProximaAplicacionRaw>> ObtenerProximasAplicaciones(
            int? idRodeo, DateTime desde, DateTime hasta)
        {
            var query =
                from ev in _context.EventosSanitarios
                join a in _context.Animales on ev.IdAnimal equals a.Id
                join u in _context.Usuarios on ev.IdUsuario equals u.Id
                where ev.FechaProximaAplicacion.HasValue
                      && ev.FechaProximaAplicacion.Value.Date >= desde
                      && ev.FechaProximaAplicacion.Value.Date <= hasta
                      && (!idRodeo.HasValue || a.RodeoId == idRodeo.Value)
                      && a.Estado
                orderby ev.FechaProximaAplicacion, a.CaravanaCuig, a.CaravanaNroManejo
                select new ProximaAplicacionRaw
                {
                    IdEvento = ev.Id,
                    Destino = a.CaravanaCuig + "-" + a.CaravanaNroManejo,
                    TipoEvento = ev.TipoEvento,
                    FechaProximaAplicacion = ev.FechaProximaAplicacion!.Value,
                    Responsable = u.Nombre
                };

            var resultados = await query.ToListAsync();
            await CompletarProductos(resultados.Cast<ReporteEventoRawBase>().ToList());
            return resultados;
        }

        public async Task<List<VencimientoRaw>> ObtenerVencimientos(
            int? idRodeo, DateTime desde, DateTime hasta)
        {
            var query =
                from ev in _context.EventosSanitarios
                join a in _context.Animales on ev.IdAnimal equals a.Id
                join u in _context.Usuarios on ev.IdUsuario equals u.Id
                where ev.VigenciaHasta.HasValue
                      && ev.VigenciaHasta.Value.Date >= desde
                      && ev.VigenciaHasta.Value.Date <= hasta
                      && (!idRodeo.HasValue || a.RodeoId == idRodeo.Value)
                      && a.Estado
                orderby ev.VigenciaHasta, a.CaravanaCuig, a.CaravanaNroManejo
                select new VencimientoRaw
                {
                    IdEvento = ev.Id,
                    Destino = a.CaravanaCuig + "-" + a.CaravanaNroManejo,
                    TipoEvento = ev.TipoEvento,
                    VigenciaHasta = ev.VigenciaHasta!.Value,
                    Responsable = u.Nombre,
                    Observaciones = ev.Observaciones
                };

            var resultados = await query.ToListAsync();
            await CompletarProductos(resultados.Cast<ReporteEventoRawBase>().ToList());
            return resultados;
        }

        private async Task CompletarProductos(List<ReporteEventoRawBase> filas)
        {
            if (filas.Count == 0) return;

            var idsEventos = filas.Select(f => f.IdEvento).Distinct().ToList();

            var detalles = await (
                from d in _context.DetallesMedicamento
                join m in _context.Medicamentos on d.IdMedicamento equals m.Id
                where idsEventos.Contains(d.IdEventoSanitario)
                select new { d.IdEventoSanitario, m.Nombre }
            ).ToListAsync();

            var productosPorEvento = detalles
                .GroupBy(d => d.IdEventoSanitario)
                .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.Nombre)));

            foreach (var fila in filas)
                fila.Producto = productosPorEvento.TryGetValue(fila.IdEvento, out var nombres)
                    ? nombres
                    : string.Empty;
        }
    }

    public abstract class ReporteEventoRawBase
    {
        public int IdEvento { get; set; }
        public string Destino { get; set; } = string.Empty;
        public string TipoEvento { get; set; } = string.Empty;
        public string Responsable { get; set; } = string.Empty;
        public string Producto { get; set; } = string.Empty;
    }

    public class ProximaAplicacionRaw : ReporteEventoRawBase
    {
        public DateTime FechaProximaAplicacion { get; set; }
    }

    public class VencimientoRaw : ReporteEventoRawBase
    {
        public DateTime VigenciaHasta { get; set; }
        public string? Observaciones { get; set; }
    }
}