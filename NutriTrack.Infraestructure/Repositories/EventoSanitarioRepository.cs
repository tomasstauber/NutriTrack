using System.Collections.Generic;
using System.Threading.Tasks;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{
    public class EventoSanitarioRepository
    {
        private readonly AppDbContext _context;

        public EventoSanitarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EventoSanitario> AgregarAsync(EventoSanitario evento)
        {
            await _context.EventosSanitarios.AddAsync(evento);
            await _context.SaveChangesAsync();
            return evento;
        }
    }
}