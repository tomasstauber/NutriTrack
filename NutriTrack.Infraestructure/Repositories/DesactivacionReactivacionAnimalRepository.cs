using System;
using System.Collections.Generic;
using NutriTrack.Infraestructure.Data;
using NutriTrack.Core.Entities;
using System.Text;
using Microsoft.EntityFrameworkCore;


namespace NutriTrack.Infraestructure.Repositories
{
    public class DesactivacionReactivacionAnimalRepository
    {
        private readonly AppDbContext _context;

        public DesactivacionReactivacionAnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Animal?> BuscarPorCaravana(string cuig , string nroManejo)
        {
            return await _context.Animales
                .Include(a => a.Rodeo)
                .Include(a => a.Madre)
                .Include(a => a.Padre)
                .FirstOrDefaultAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nroManejo);
        }

        public async Task Actualizar(Animal animal)
        {
            await _context.SaveChangesAsync();
        }
    }
}
