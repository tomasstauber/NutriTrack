using System;
using System.Collections.Generic;

namespace NutriTrack.Core.Reportes
{
    // Dado un tipo de reporte (o una cantidad de dias personalizada) y una
    // fecha de referencia (normalmente "hoy"), devuelve el rango [desde, hasta]
    // contando hacia atras, INCLUSIVE de la fecha de referencia.
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

        // Atajo para los tipos: actual, diario, semanal,
        // quincenal, mensual, anual. Todos cuentan hacia atras desde fechaReferencia.
        // "personalizado" NO pasa por aca: usa un rango [desde, hasta] 
        // que el usuario elige directo, sin calculo de dias. Ver Controller.
        public static (DateTime Desde, DateTime Hasta) Calcular(string tipoReporte, DateTime fechaReferencia)
        {
            if (!DiasPorTipo.TryGetValue(tipoReporte.ToLower(), out var dias))
                throw new ArgumentException($"tipo_reporte invalido: {tipoReporte}");

            return CalcularPorCantidadDias(dias, fechaReferencia);
        }

        // Generico: cuenta "cantidadDias" hacia atras desde fechaReferencia, INCLUSIVE.
        // Ej: fechaReferencia=8/9, cantidadDias=3 => (6/9, 8/9)
        // No lo usa CU17 (personalizado ahi usa un rango explicito), pero queda
        // disponible como utilidad reutilizable para otros reportes (CU18/19/20).
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