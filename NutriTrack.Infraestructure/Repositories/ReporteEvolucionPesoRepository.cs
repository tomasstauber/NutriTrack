using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace NutriTrack.Infraestructure.Repositories
{

    public class ReporteEvolucionPesoRepository
    {
        private readonly AppDbContext _context;

        public ReporteEvolucionPesoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteRodeo(int idRodeo)
        {
            return await _context.Rodeos.AnyAsync(r => r.Id == idRodeo && r.Activo);
        }

        
        public async Task<Animal?> BuscarAnimalPorCaravana(string caravana)
        {
            var partes = caravana.Split('-', 2);
            var cuig = partes[0];
            var nro = partes.Length > 1 ? partes[1] : string.Empty;

            return await _context.Animales
                .FirstOrDefaultAsync(a => a.CaravanaCuig == cuig && a.CaravanaNroManejo == nro);
        }
        public async Task<List<AnimalEvolucionPesoRaw>> ObtenerResumenPorAnimal(
            int? idRodeo, DateTime desde, DateTime hasta, int? idAnimalFiltro)
        {
            var query =
                from rp in _context.RegistrosPeso
                join a in _context.Animales on rp.IdAnimal equals a.Id
                where rp.FechaPesaje.Date >= desde
                      && rp.FechaPesaje.Date <= hasta
                      && (!idRodeo.HasValue || a.RodeoId == idRodeo.Value)
                      && (!idAnimalFiltro.HasValue || a.Id == idAnimalFiltro.Value)
                select new { rp.FechaPesaje, rp.PesoKg, a.Id, a.CaravanaCuig, a.CaravanaNroManejo };

            var filas = await query.ToListAsync();
            return filas
                .GroupBy(x => x.Id)
                .Select(g =>
                {
                    var ordenado = g.OrderBy(x => x.FechaPesaje).ToList();
                    var primero = ordenado.First();
                    var ultimo = ordenado.Last();

                    return new AnimalEvolucionPesoRaw
                    {
                        Caravana = $"{primero.CaravanaCuig}-{primero.CaravanaNroManejo}",
                        FechaInicial = primero.FechaPesaje,
                        PesoInicial = primero.PesoKg,
                        FechaFinal = ultimo.FechaPesaje,
                        PesoFinal = ultimo.PesoKg,
                        CantidadRegistros = ordenado.Count,
                        
                        VariacionKg = ordenado.Count >= 2 ? (float?)(ultimo.PesoKg - primero.PesoKg) : null
                    };
                })
                .OrderBy(r => r.Caravana)
                .ToList();
        }

        
        public async Task<List<PesajeDetalleRaw>> ObtenerDetallePesajes(
            int idAnimal, DateTime desde, DateTime hasta)
        {
            return await _context.RegistrosPeso
                .Where(r => r.IdAnimal == idAnimal
                            && r.FechaPesaje.Date >= desde
                            && r.FechaPesaje.Date <= hasta)
                .OrderBy(r => r.FechaPesaje)
                .Select(r => new PesajeDetalleRaw { FechaPesaje = r.FechaPesaje, PesoKg = r.PesoKg })
                .ToListAsync();
        }
    }

    public class AnimalEvolucionPesoRaw
    {
        public string Caravana { get; set; } = string.Empty;
        public DateTime FechaInicial { get; set; }
        public float PesoInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public float PesoFinal { get; set; }
        public float? VariacionKg { get; set; }
        public int CantidadRegistros { get; set; }
    }

    public class PesajeDetalleRaw
    {
        public DateTime FechaPesaje { get; set; }
        public float PesoKg { get; set; }
    }
}