using System;
using System.Collections.Generic;

namespace NutriTrack.API.DTOs
{
    public class AlertaResponseDTO
    {
        public int DiasAviso { get; set; }
        // Días hacia atrás que se miran los pesajes para el desvío de peso
        public int DiasPeriodoPeso { get; set; }
        public int Total { get; set; }
        public List<AlertaItemDTO> Alertas { get; set; } = new();
    }

    public class AlertaItemDTO
    {
        // "Sanitaria" / "PlanAlimenticio" / "DesvioPeso"
        public string Tipo { get; set; }
        // "ProximaAplicacion" / "Vencimiento" / "PorAnimal" (desvío de peso)
        public string Subtipo { get; set; }
        // Caravana del animal o nombre del rodeo
        public string Destino { get; set; }
        public string Descripcion { get; set; }
        public DateOnly Fecha { get; set; }
        // Fecha - hoy. En el desvío de peso, Fecha es la del último pesaje: 0 o negativo (hace N días)
        public int DiasRestantes { get; set; }
        // Solo planes; null en las sanitarias
        public DateOnly? VigenciaDesde { get; set; }
        public int? CantidadAnimales { get; set; }
        // Solo desvío de peso; null en las demás. kg por día; la real con 2 decimales
        public decimal? GananciaReal { get; set; }
        public decimal? GananciaEsperada { get; set; }
        // Solo desvío de peso: fechas del pesaje anterior y del último, entre las que se calculó la ganancia
        public DateOnly? PeriodoDesde { get; set; }
        public DateOnly? PeriodoHasta { get; set; }
    }
}
