using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoSanitarioController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EventoSanitarioController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventoSanitario>>> ObtenerTodos()
        {
            var eventos = await _context.EventosSanitarios.ToListAsync();

            return Ok(eventos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EventoSanitario>> ObtenerPorId(int id)
        {
            var evento = await _context.EventosSanitarios.FindAsync(id);

            if (evento == null)
            {
                return NotFound();
            }

            return Ok(evento);
        }

        [HttpPost]
        public async Task<ActionResult<EventoSanitario>> Crear(EventoSanitario evento)
        {
            _context.EventosSanitarios.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(ObtenerPorId),
                new { id = evento.Id },
                evento);
        }
    }
}


