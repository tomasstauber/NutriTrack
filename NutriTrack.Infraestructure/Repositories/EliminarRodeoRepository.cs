using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class EliminarRodeoRepository
    {
        private readonly AppDbContext _context;

        public EliminarRodeoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ContarPlanesPorRodeo(int idRodeo)
        {
            return await _context.PlanRodeoAsignacions
                .CountAsync(p => p.IdRodeo == idRodeo);
        }

        public async Task EliminarRodeo(int idRodeo)
        {
            await _context.Animales
                .Where(a => a.RodeoId == idRodeo)
                .ExecuteUpdateAsync(a => a.SetProperty(x => x.RodeoId, (int?)null));

            await _context.PlanRodeoAsignacions
                .Where(p => p.IdRodeo == idRodeo)
                .ExecuteDeleteAsync();

            await _context.Rodeos
                .Where(r => r.Id == idRodeo)
                .ExecuteDeleteAsync();
        }
    }
}
