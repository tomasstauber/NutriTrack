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
            return await _context.Rodeos.AnyAsync(r => r.Id == idRodeo && r.Activo);
        }

        // Tipo "actual": todo el stock activo del alcance, sin filtrar por fecha de alta
        public async Task<List<Animal>> ObtenerInventario(int? idRodeo)
        {
            return await OrdenarPorCaravana(ConsultaBase(idRodeo)).ToListAsync();
        }

        // Resto de los tipos: animales activos dados de alta en el período
        public async Task<List<Animal>> ObtenerInventario(int? idRodeo, DateTime desde, DateTime hasta)
        {
            var query = ConsultaBase(idRodeo)
                .Where(a => a.FechaAlta.Date >= desde && a.FechaAlta.Date <= hasta);

            return await OrdenarPorCaravana(query).ToListAsync();
        }

        // Activos, con su rodeo, y del rodeo elegido si viene
        private IQueryable<Animal> ConsultaBase(int? idRodeo)
        {
            var query = _context.Animales
                .Include(a => a.Rodeo)
                .Where(a => a.Estado);

            if (idRodeo.HasValue)
                query = query.Where(a => a.RodeoId == idRodeo.Value);

            return query;
        }

        // El JSON y el PDF salen en el mismo orden
        private static IQueryable<Animal> OrdenarPorCaravana(IQueryable<Animal> query) =>
            query.OrderBy(a => a.CaravanaCuig).ThenBy(a => a.CaravanaNroManejo);
    }
}