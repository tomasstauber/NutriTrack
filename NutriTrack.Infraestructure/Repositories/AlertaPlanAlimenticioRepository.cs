using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    public class AlertaPlanAlimenticioRepository
    {
        private readonly AppDbContext _context;

        public AlertaPlanAlimenticioRepository(AppDbContext context)
        {
            _context = context;
        }

        // Asignaciones activas, de rodeos activos, cuya vigencia hasta cae en [desde, hasta].
        // CantidadAnimales cuenta solo los activos y se resuelve en la misma consulta
        public async Task<List<VencimientoPlanRaw>> ObtenerVencimientos(DateOnly desde, DateOnly hasta)
        {
            return await _context.PlanRodeoAsignacions
                .Where(a => a.Activo
                            && a.VigenciaHasta.HasValue
                            && a.VigenciaHasta.Value >= desde
                            && a.VigenciaHasta.Value <= hasta
                            && a.Rodeo!.Activo)
                .OrderBy(a => a.VigenciaHasta)
                .ThenBy(a => a.Rodeo!.Nombre)
                .Select(a => new VencimientoPlanRaw
                {
                    IdAsignacion = a.Id,
                    NombrePlan = a.PlanAlimenticio!.NombrePlan,
                    NombreRodeo = a.Rodeo!.Nombre,
                    VigenciaDesde = a.VigenciaDesde,
                    VigenciaHasta = a.VigenciaHasta!.Value,
                    CantidadAnimales = a.Rodeo.Animales.Count(an => an.Estado)
                })
                .ToListAsync();
        }
    }

    public class VencimientoPlanRaw
    {
        public int IdAsignacion { get; set; }
        public string NombrePlan { get; set; } = string.Empty;
        public string NombreRodeo { get; set; } = string.Empty;
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly VigenciaHasta { get; set; }
        public int CantidadAnimales { get; set; }
    }
}
