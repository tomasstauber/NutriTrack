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
}
