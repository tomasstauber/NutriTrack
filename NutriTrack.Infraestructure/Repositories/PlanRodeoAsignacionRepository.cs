using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class PlanRodeoAsignacionRepository
    {
        private readonly AppDbContext _context;

        public PlanRodeoAsignacionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PlanRodeoAsignacion?> ObtenerAsignacionActivaPorRodeo(int IdRodeo)
        {
           return await _context.PlanRodeoAsignacions
                .Include(a => a.PlanAlimenticio)
                .FirstOrDefaultAsync(a => a.IdRodeo == IdRodeo && a.Activo);
        }

        // Asignaciones activas de un plan, con los datos del rodeo.
        // CantidadAnimales cuenta solo los activos, igual que el listado de rodeos
        public async Task<List<AsignacionActivaPlanRaw>> ListarActivasPorPlan(int IdPlan)
        {
            return await _context.PlanRodeoAsignacions
                .Where(a => a.IdPlanAlimenticio == IdPlan && a.Activo)
                .OrderBy(a => a.Rodeo!.Nombre)
                .Select(a => new AsignacionActivaPlanRaw
                {
                    IdAsignacion = a.Id,
                    IdRodeo = a.IdRodeo,
                    NombreRodeo = a.Rodeo!.Nombre,
                    DescripcionRodeo = a.Rodeo.Descripcion,
                    CantidadAnimales = a.Rodeo.Animales.Count(an => an.Estado),
                    VigenciaDesde = a.VigenciaDesde,
                    VigenciaHasta = a.VigenciaHasta
                })
                .ToListAsync();
        }

        public async Task AsignarAsync(PlanRodeoAsignacion asignacion)
        {
            await _context.PlanRodeoAsignacions.AddAsync(asignacion);
            await _context.SaveChangesAsync();
        }

        public async Task ActualizarAsignacionAsync(PlanRodeoAsignacion asignacion)
        {
            await _context.SaveChangesAsync();
        }
    }

    public class AsignacionActivaPlanRaw
    {
        public int IdAsignacion { get; set; }
        public int IdRodeo { get; set; }
        public string NombreRodeo { get; set; } = string.Empty;
        public string? DescripcionRodeo { get; set; }
        public int CantidadAnimales { get; set; }
        public DateOnly VigenciaDesde { get; set; }
        public DateOnly? VigenciaHasta { get; set; }
    }
}
