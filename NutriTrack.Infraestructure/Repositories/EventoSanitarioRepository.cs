using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    public class EventoSanitarioRepository
    {
        private readonly AppDbContext _context;

        public EventoSanitarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EventoSanitario>> ObtenerEventosProximosAsync(
            DateTime desde,
            DateTime hasta)
        {
            return await _context.EventosSanitarios
                .Where(e =>
                    (e.FechaProximaAplicacion.HasValue &&
                     e.FechaProximaAplicacion.Value >= desde &&
                     e.FechaProximaAplicacion.Value <= hasta)
                    ||
                    (e.VigenciaHasta.HasValue &&
                     e.VigenciaHasta.Value >= desde &&
                     e.VigenciaHasta.Value <= hasta)
                )
                .ToListAsync();
        }
    }
}