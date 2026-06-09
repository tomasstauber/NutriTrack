using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class PlanAlimenticioRepository
    {
        private readonly AppDbContext _context;

        public PlanAlimenticioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearPlanAlimenticioAsync(PlanAlimenticio plan)
        {
            await _context.PlanesAlimenticios.AddAsync(plan);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerificarNombreUnico(string NombrePlan)
        {
            return await _context.PlanesAlimenticios
                .AnyAsync(p => p.NombrePlan == NombrePlan);
        }

        public async Task<PlanAlimenticio?> BuscarPlanId(int IdPlan)
        {
            return await _context.PlanesAlimenticios
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.Id == IdPlan);
        }
    }
}
