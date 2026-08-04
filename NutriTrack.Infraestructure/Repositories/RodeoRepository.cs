using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class RodeoRepository
    {
        private readonly AppDbContext _context;

        public RodeoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteNombre(string nombre)
        {
            return await _context.Rodeos.AnyAsync(r => r.Nombre == nombre);
        }

        public async Task<Rodeo> Crear(Rodeo rodeo)
        {
            _context.Rodeos.Add(rodeo);
            await _context.SaveChangesAsync();
            return rodeo;
        }

        public async Task<Rodeo?> BuscarPorId(int idRodeo)
        {
            return await _context.Rodeos.FirstOrDefaultAsync(r => r.Id == idRodeo);
        }
    }
}