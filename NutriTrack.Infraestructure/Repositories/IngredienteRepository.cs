using NutriTrack.Infraestructure.Data;
using NutriTrack.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class IngredienteRepository
    {
        private readonly AppDbContext _context;

        public IngredienteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CrearAsync(Ingrediente ingrediente)
        {
            await _context.Ingredientes.AddAsync(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ingrediente>> ObtenerTodosAsync()
        {
            return await _context.Ingredientes
                .Where(i => i.Activo)
                .ToListAsync();
        }

        public async Task<Ingrediente?> BuscarPorId(int IdIngrediente)
        {
            return await _context.Ingredientes
                .FirstOrDefaultAsync(i => i.Id == IdIngrediente);
        }

        public async Task<List<Ingrediente>> BuscarPorNombre(string NombreIngrdiente)
        {
            return await _context.Ingredientes
                .Where(i => i.NombreIngrediente.ToLower().Contains(NombreIngrdiente.ToLower()))
                .Where(i => i.Activo)
                .ToListAsync();
        }

        public async Task<bool> VerificarNombreUnico(string NombreIngrediente)
        {
            return await _context.Ingredientes
                .AnyAsync(i => i.NombreIngrediente.ToLower() == NombreIngrediente.ToLower() && i.Activo);
        }

        public async Task ActualizarAsync(Ingrediente ingrediente)
        {
            _context.Ingredientes.Update(ingrediente);
            await _context.SaveChangesAsync();
        }

        public async Task DesactivarAsync(Ingrediente ingrediente)
        {
            ingrediente.Activo = false;
            await _context.SaveChangesAsync();
        }

        public async Task<List<PlanAlimenticio>> ObtenerPlanesQueUsan(int IdIngrediente)
        {
            return await _context.PlanAlimenticioDetalles
                .Where(d => d.IdIngrediente == IdIngrediente && d.PlanAlimenticio != null)
                .Select(d => d.PlanAlimenticio)
                .Distinct()
                .ToListAsync();
        } 
    }
}