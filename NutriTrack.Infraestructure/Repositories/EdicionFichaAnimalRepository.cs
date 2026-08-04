using System;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class EdicionFichaAnimalRepository
    {
        private readonly AppDbContext _context;

        public EdicionFichaAnimalRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task<bool> ExisteCaravana(string cuig, string nroManejo) 
        {
            return await _context.Animales
                .AnyAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);

        }

        public async Task<Animal?> BuscarPorCaravana (string cuig, string nroManejo)
        {
            return await _context.Animales
                .FirstOrDefaultAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }

        public async Task<Animal> Actualizar(Animal animal)
        {
            _context.Entry(animal).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return animal;
        }

    }
}
