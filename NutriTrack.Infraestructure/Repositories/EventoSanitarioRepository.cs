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

        public async Task<List<EventoHistorialRaw>> ObtenerPorAnimal(int idAnimal)
        {
            return await _context.EventosSanitarios
                .Where(e => e.IdAnimal == idAnimal)
                .OrderByDescending(e => e.FechaEvento)
                .Select(e => new EventoHistorialRaw
                {
                    Id = e.Id,
                    TipoEvento = e.TipoEvento,
                    FechaEvento = e.FechaEvento,
                    VigenciaHasta = e.VigenciaHasta,
                    FechaProximaAplicacion = e.FechaProximaAplicacion,
                    Observaciones = e.Observaciones,
                    Responsable = _context.Usuarios
                        .Where(u => u.Id == e.IdUsuario)
                        .Select(u => u.NombreUsuario)
                        .FirstOrDefault(),
                    Medicamentos = e.DetallesMedicamento
                        .Select(d => new MedicamentoHistorialRaw
                        {
                            Nombre = _context.Medicamentos
                                .Where(m => m.Id == d.IdMedicamento)
                                .Select(m => m.Nombre)
                                .FirstOrDefault(),
                            Dosis = d.Dosis,
                            Unidad = d.Unidad,
                            Observaciones = d.Observaciones
                        })
                        .ToList()
                })
                .ToListAsync();
        }
    }

    public class EventoHistorialRaw
    {
        public int Id { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
        public DateTime? VigenciaHasta { get; set; }
        public DateTime? FechaProximaAplicacion { get; set; }
        public string? Observaciones { get; set; }
        public string? Responsable { get; set; }
        public List<MedicamentoHistorialRaw> Medicamentos { get; set; } = new();
    }

    public class MedicamentoHistorialRaw
    {
        public string? Nombre { get; set; }
        public decimal? Dosis { get; set; }
        public string? Unidad { get; set; }
        public string? Observaciones { get; set; }
    }
}