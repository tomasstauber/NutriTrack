using Microsoft.EntityFrameworkCore;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Services
{
    public class AlertaPesoService
    {
        private readonly AppDbContext _context;

        public AlertaPesoService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> DetectarDesviosAsync()
        {
            var alertas = new List<string>();

            var animales = await _context.Animales
                .Where(a => a.Estado && a.RodeoId.HasValue)
                .ToListAsync();

            foreach (var animal in animales)
            {
                var pesajes = await _context.RegistrosPeso
                    .Where(p => p.IdAnimal == animal.Id)
                    .OrderByDescending(p => p.FechaPesaje)
                    .Take(2)
                    .ToListAsync();

                if (pesajes.Count < 2)
                    continue;

                var ultimo = pesajes[0];
                var anterior = pesajes[1];

                var dias = (ultimo.FechaPesaje - anterior.FechaPesaje).TotalDays;

                if (dias <= 0)
                    continue;

                var gananciaDiaria =
                    (ultimo.PesoKg - anterior.PesoKg) / (float)dias;

                var plan = await _context.PlanesAlimenticios
                    .FirstOrDefaultAsync();

                if (plan == null || !plan.GananciaPesoEsperada.HasValue)
                    continue;

                if ((decimal)gananciaDiaria < plan.GananciaPesoEsperada.Value)
                {
                    alertas.Add(
                        $"Alerta: el animal {animal.CaravanaNroManejo} " +
                        $"presenta una ganancia de peso inferior a la esperada."
                    );
                }
            }

            return alertas;
        }
    }
}