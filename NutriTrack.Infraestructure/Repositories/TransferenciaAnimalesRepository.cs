using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace NutriTrack.Infraestructure.Repositories
{
    public class TransferenciaAnimalesRepository
    {
        private readonly AppDbContext _context;
        public TransferenciaAnimalesRepository(AppDbContext context)
        {
            _context = context;
        }

        //ObtenerAnimalesPorRodeo(int idRodeo)
        public async Task<List<Animal>> ObtenerAnimalesPorRodeo(int IdRodeo)
        {
            return await _context.Animales
                 .Include(a => a.Rodeo)
                 .Where(a => a.RodeoId == IdRodeo && a.Estado)
                 .ToListAsync();
        }
        //ObtenerAnimalPorId(int idAnimal)
        public async Task <Animal?> ObtenerAnimalPorId (int IdAnimal)
        {
            return await _context.Animales
                .FirstOrDefaultAsync(a => a.Id == IdAnimal && a.Estado);
        }

        //Actualizar(Animal animal)
        public async Task GuardarCambios()
        {
            await _context.SaveChangesAsync();
        }
    }
}
