using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class AltaAnimalRepository
    {
        private readonly AppDbContext _context;

        public AltaAnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteCaravana(string cuig, string nroManejo)
        {
            return await _context.Animales
                .AnyAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }

        public async Task<Animal?> BuscarPorCaravana (string cuig, string nroManejo)
        {  return await _context.Animales
                .FirstOrDefaultAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }
   
        public async Task<Animal> Crear(Animal animal)
        {
            _context.Animales.Add(animal);
            await _context.SaveChangesAsync();
            return animal;
        }

    }
}

