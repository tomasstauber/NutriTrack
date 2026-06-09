using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    public class AnimalRepository
    {
        private readonly AppDbContext _context;

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Animal> ObtenerActivosSinRodeo(string? caravana = null)
        {
            var query = _context.Animales
                .Where(a => a.Estado && a.RodeoId == null);

            if (!string.IsNullOrEmpty(caravana))
                query = query.Where(a =>
                    a.CaravanaCuig.Contains(caravana) ||
                    a.CaravanaNroManejo.Contains(caravana));

            return query.ToList();
        }

        public List<Animal> ObtenerPorIds(List<int> ids)
        {
            return _context.Animales
                .Where(a => ids.Contains(a.Id) && a.Estado && a.RodeoId == null)
                .ToList();
        }
    }
}