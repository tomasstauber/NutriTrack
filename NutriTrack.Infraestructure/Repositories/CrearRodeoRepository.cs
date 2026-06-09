using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    public class CrearRodeoRepository
    {
        private readonly AppDbContext _context;

        public CrearRodeoRepository(AppDbContext context)
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
    }
}