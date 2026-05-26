using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class RegistroPesoRepository
    {
        private readonly AppDbContext _context;

        public RegistroPesoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarAsync(RegistroPeso registro)
        {
            registro.FechaPesaje = DateTime.SpecifyKind(registro.FechaPesaje, DateTimeKind.Utc);
            await _context.RegistrosPeso.AddAsync(registro);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RegistroPeso>> ObtenerPorAnimalAsync(int idAnimal)
        {
            return await _context.RegistrosPeso
                .Where(r => r.IdAnimal == idAnimal)
                .OrderByDescending(r => r.FechaPesaje)
                .ToListAsync();
        }
    }
}
