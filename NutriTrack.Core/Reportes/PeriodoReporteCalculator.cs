using System;
using System.Collections.Generic;

namespace NutriTrack.Core.Reportes
{
    
    public static class PeriodoReporteCalculator
    {
        private static readonly Dictionary<string, int> DiasPorTipo = new()
        {
            ["actual"] = 1,
            ["diario"] = 1,
            ["semanal"] = 7,
            ["quincenal"] = 15,
            ["mensual"] = 30,
            ["anual"] = 365
        };

        // quincenal, mensual, anual. Todos cuentan hacia atras desde fechaReferencia.
        // "personalizado" NO pasa por aca: usa un rango [desde, hasta] explicito
        // que el usuario elige directo, sin calculo de dias. Ver Controller.
        public static (DateTime Desde, DateTime Hasta) Calcular(string tipoReporte, DateTime fechaReferencia)
        {
            if (!DiasPorTipo.TryGetValue(tipoReporte.ToLower(), out var dias))
                throw new ArgumentException($"tipo_reporte invalido: {tipoReporte}");

            return CalcularPorCantidadDias(dias, fechaReferencia);
        }
   
        // Ej: fechaReferencia=8/9, cantidadDias=3 => (6/9, 8/9)
        public static (DateTime Desde, DateTime Hasta) CalcularPorCantidadDias(int cantidadDias, DateTime fechaReferencia)
        {
            if (cantidadDias < 1)
                throw new ArgumentException("La cantidad de dias debe ser mayor a 0");

            var hasta = fechaReferencia.Date;
            var desde = hasta.AddDays(-(cantidadDias - 1));
            return (desde, hasta);
        }

        // "personalizado" se valida como tipo aceptado, pero no tiene una cantidad
        // de dias fija: el usuario aporta desde/hasta directo (ver Controller).
        public static readonly string[] TiposValidos =
            { "actual", "diario", "semanal", "quincenal", "mensual", "anual", "personalizado" };
    }
}