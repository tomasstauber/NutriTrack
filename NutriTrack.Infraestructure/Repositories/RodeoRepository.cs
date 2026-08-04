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

        public bool ExisteNombre(string nombre)
        {
            return _context.Rodeos.Any(r => r.Nombre == nombre);
        }

        public Rodeo Crear(Rodeo rodeo)
        {
            _context.Rodeos.Add(rodeo);
            _context.SaveChanges();
            return rodeo;
        }

        public async Task<Rodeo?> BuscarPorId(int idRodeo)
        {
            return await _context.Rodeos.FirstOrDefaultAsync(r => r.Id == idRodeo);
        }
    }
}