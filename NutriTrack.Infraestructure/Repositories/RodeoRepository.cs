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
            return await _context.Rodeos
                .AnyAsync(r => r.Activo && r.Nombre.ToLower() == nombre.ToLower());
        }

        public async Task<Rodeo> Crear(Rodeo rodeo)
        {
            _context.Rodeos.Add(rodeo);
            await _context.SaveChangesAsync();
            return rodeo;
        }

        public async Task<Rodeo?> BuscarPorId(int idRodeo)
        {
            return await _context.Rodeos.FirstOrDefaultAsync(r => r.Id == idRodeo && r.Activo);
        }

        public async Task<List<RodeoListadoRaw>> ListarRodeosActivos()
        {
            return await _context.Rodeos
                .Where(r => r.Activo)
                .OrderBy(r => r.Nombre)
                .Select(r => new RodeoListadoRaw
                {
                    Id = r.Id,
                    Nombre = r.Nombre,
                    Descripcion = r.Descripcion,
                    CantidadAnimales = r.Animales.Count(a => a.Estado)
                })
                .ToListAsync();
        }
    }

    public class RodeoListadoRaw
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int CantidadAnimales { get; set; }
    }
}