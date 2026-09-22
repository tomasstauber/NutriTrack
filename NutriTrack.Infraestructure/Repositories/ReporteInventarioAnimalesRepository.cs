using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    public class ReporteInventarioAnimalesRepository
    {
        private readonly AppDbContext _context;

        public ReporteInventarioAnimalesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteRodeo(int idRodeo)
        {
            return await _context.Rodeos.AnyAsync(r => r.Id == idRodeo);
        }

        public async Task<List<Animal>> ObtenerInventario(int? idRodeo, DateTime desde, DateTime hasta)
        {
            var query = _context.Animales
                .Include(a => a.Rodeo)
                .Where(a => a.Estado); 

            if (idRodeo.HasValue)
                query = query.Where(a => a.RodeoId == idRodeo.Value); 

            query = query.Where(a => a.FechaAlta.Date >= desde && a.FechaAlta.Date <= hasta);

            return await query.ToListAsync();
        }
    }
}