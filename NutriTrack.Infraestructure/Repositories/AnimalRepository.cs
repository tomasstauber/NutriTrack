using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace NutriTrack.Infraestructure.Repositories
{
    public class AnimalRepository
    {
        private readonly AppDbContext _context;

        public AnimalRepository(AppDbContext context)
        {
            _context = context;
        }

        // Devuelve la página pedida y el total de coincidencias (para que el
        // front sepa cuántas páginas hay).
        public async Task<(List<Animal> Items, int Total)> Listar(
            string? texto,
            int? idRodeo,
            bool sinRodeo,
            bool incluirInactivos,
            int pagina,
            int tamanioPagina)
        {
     
            var query = _context.Animales
                .AsNoTracking()
                .Include(a => a.Rodeo)
                .AsQueryable();

            if (!incluirInactivos)
                query = query.Where(a => a.Estado);

            if (sinRodeo)
                query = query.Where(a => a.RodeoId == null);
            else if (idRodeo.HasValue)
                query = query.Where(a => a.RodeoId == idRodeo.Value);

            if (!string.IsNullOrWhiteSpace(texto))
            {
                // ILIKE: coincidencia parcial sin distinguir mayúsculas (PostgreSQL).
                // Concatenar las dos partes permite buscar por cuig, por número
                // de manejo o por la caravana completa.
                var patron = $"%{texto.Trim()}%";
                query = query.Where(a =>
                    EF.Functions.ILike(a.CaravanaCuig + a.CaravanaNroManejo, patron) ||
                    EF.Functions.ILike(a.Raza, patron));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(a => a.CaravanaCuig)
                .ThenBy(a => a.CaravanaNroManejo)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return (items, total);
        }

        public async Task<List<Animal>> ObtenerActivosSinRodeo(string? caravana = null)
        {
            var query = _context.Animales
                .Where(a => a.Estado && a.RodeoId == null);

            if (!string.IsNullOrEmpty(caravana))
                query = query.Where(a =>
                    a.CaravanaCuig.Contains(caravana) ||
                    a.CaravanaNroManejo.Contains(caravana));

            return await query.ToListAsync();
        }

        public async Task<int> ContarActivosPorRodeo(int IdRodeo)
        {
            return await _context.Animales
                .CountAsync(a => a.RodeoId == IdRodeo && a.Estado);
        }

        public async Task<List<Animal>> ObtenerPorIds(List<int> ids)
        {
            return await _context.Animales
                .Where(a => ids.Contains(a.Id) && a.Estado && a.RodeoId == null)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObtenerActivosPorRodeo(int idRodeo)
        {
            return await _context.Animales
                .Where(a => a.RodeoId == idRodeo && a.Estado)
                .ToListAsync();
        }

        public async Task<List<Animal>> ObtenerTodosActivos()
        {
            return await _context.Animales
                .Where(a => a.Estado)
                .ToListAsync();
        }

        public async Task<Animal?> ObtenerActivoPorId(int idAnimal)
        {
            return await _context.Animales
                .FirstOrDefaultAsync(a => a.Id == idAnimal && a.Estado);
        }

        public async Task<bool> ExisteAnimalPorId(int idAnimal)
        {
            return await _context.Animales
                .AnyAsync(a => a.Id == idAnimal);
        }
    }
}