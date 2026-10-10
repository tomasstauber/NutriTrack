using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NutriTrack.Infraestructure.Data;

namespace NutriTrack.Infraestructure.Repositories
{
    // CU14: animales cuya ganancia de peso real es menor que la esperada del plan activo de su rodeo.
    // Se evalúa solo por animal (el promedio por rodeo, R3, queda como mejora futura).
    // Se usa el rodeo ACTUAL del animal: no hay historial de pertenencia animal–rodeo
    public class AlertaDesvioPesoRepository
    {
        private readonly AppDbContext _context;

        public AlertaDesvioPesoRepository(AppDbContext context)
        {
            _context = context;
        }

        // Pesajes con fecha en [desde, hasta], los dos incluidos. Una sola consulta para todos los animales:
        // el agrupamiento y el cálculo se hacen en memoria
        public async Task<List<DesvioPesoRaw>> ObtenerDesvios(DateTime desde, DateTime hasta)
        {
            // No se evalúan: animales inactivos, sin rodeo (el join lo descarta), rodeos inactivos,
            // rodeos sin asignación activa ni planes sin ganancia esperada (R5).
            // El índice único garantiza una sola asignación activa por rodeo: cada pesaje aparece una vez
            var pesajes = await (
                from rp in _context.RegistrosPeso
                join a in _context.Animales on rp.IdAnimal equals a.Id
                join asig in _context.PlanRodeoAsignacions on a.RodeoId equals (int?)asig.IdRodeo
                where rp.FechaPesaje >= desde
                      && rp.FechaPesaje <= hasta
                      && a.Estado
                      && asig.Activo
                      && asig.Rodeo!.Activo
                      && asig.PlanAlimenticio!.GananciaPesoEsperada != null
                select new
                {
                    rp.Id,
                    rp.IdAnimal,
                    rp.FechaPesaje,
                    rp.PesoKg,
                    a.CaravanaCuig,
                    a.CaravanaNroManejo,
                    asig.PlanAlimenticio!.NombrePlan,
                    GananciaEsperada = asig.PlanAlimenticio.GananciaPesoEsperada!.Value
                })
                .ToListAsync();

            var desvios = new List<DesvioPesoRaw>();

            foreach (var grupo in pesajes.GroupBy(p => p.IdAnimal))
            {
                // Más reciente primero. A igual fecha, el último cargado (Id más alto)
                var ordenados = grupo
                    .OrderByDescending(p => p.FechaPesaje)
                    .ThenByDescending(p => p.Id)
                    .ToList();

                var ultimo = ordenados[0];
                var fechaUltimo = DateOnly.FromDateTime(ultimo.FechaPesaje);

                // Anterior: el más reciente con fecha ESTRICTAMENTE anterior a la del último.
                // R1 / E1: sin dos pesajes en fechas distintas no se evalúa (y nunca se divide por cero)
                var anterior = ordenados.FirstOrDefault(p => DateOnly.FromDateTime(p.FechaPesaje) < fechaUltimo);
                if (anterior is null)
                    continue;

                var fechaAnterior = DateOnly.FromDateTime(anterior.FechaPesaje);
                var dias = fechaUltimo.DayNumber - fechaAnterior.DayNumber;

                // R2: kg por día, con decimal y sin redondear (se redondea recién al mostrar)
                var gananciaReal = (ultimo.PesoKg - anterior.PesoKg) / dias;

                // R4: alerta solo si es MENOR; igual no alerta
                if (gananciaReal >= ultimo.GananciaEsperada)
                    continue;

                desvios.Add(new DesvioPesoRaw
                {
                    Caravana = $"{ultimo.CaravanaCuig}-{ultimo.CaravanaNroManejo}",
                    NombrePlan = ultimo.NombrePlan,
                    GananciaReal = gananciaReal,
                    GananciaEsperada = ultimo.GananciaEsperada,
                    FechaPesajeAnterior = fechaAnterior,
                    FechaPesajeUltimo = fechaUltimo
                });
            }

            return desvios;
        }
    }

    public class DesvioPesoRaw
    {
        // "CUIG-NRO"
        public string Caravana { get; set; } = string.Empty;
        public string NombrePlan { get; set; } = string.Empty;
        // kg por día, sin redondear
        public decimal GananciaReal { get; set; }
        public decimal GananciaEsperada { get; set; }
        // Tramo sobre el que se calculó la ganancia
        public DateOnly FechaPesajeAnterior { get; set; }
        public DateOnly FechaPesajeUltimo { get; set; }
    }
}
