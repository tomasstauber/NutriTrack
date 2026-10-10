using System.Globalization;

namespace NutriTrack.MAUI.Models
{
    // Alerta vigente de GET api/Alerta (en el back: AlertaItemDTO)
    public class Alerta
    {
        // Los kg/día del pantallazo se escriben con coma decimal, como la descripción del back
        private static readonly CultureInfo CulturaTexto = CultureInfo.GetCultureInfo("es-AR");

        // "Sanitaria" / "PlanAlimenticio" / "DesvioPeso". Texto y no enum: un tipo nuevo del back
        // no tiene que romper la lectura de toda la lista
        public string Tipo { get; set; } = string.Empty;

        // "ProximaAplicacion" / "Vencimiento" / "PorAnimal" (desvío de peso)
        public string Subtipo { get; set; } = string.Empty;

        // Caravana "CUIG-NRO" en las sanitarias y en el desvío de peso; nombre del rodeo en las de plan
        public string Destino { get; set; } = string.Empty;

        // Medicamentos o tipo de evento en las sanitarias; nombre del plan en las de plan;
        // en el desvío de peso, el plan y las dos ganancias ("Engorde: 0,35 kg/día, esperado 0,80 kg/día")
        public string Descripcion { get; set; } = string.Empty;

        // En el desvío de peso, la fecha del último pesaje
        public DateOnly Fecha { get; set; }

        // 0 = hoy. En el desvío de peso es 0 o negativo (hace N días)
        public int DiasRestantes { get; set; }

        // Solo planes; null en las sanitarias
        public DateOnly? VigenciaDesde { get; set; }
        public int? CantidadAnimales { get; set; }

        // Solo desvío de peso; null en las demás. kg por día (la real ya viene con 2 decimales)
        public decimal? GananciaReal { get; set; }
        public decimal? GananciaEsperada { get; set; }

        // Solo desvío de peso: pesaje anterior y último, entre los que se calculó la ganancia
        public DateOnly? PeriodoDesde { get; set; }
        public DateOnly? PeriodoHasta { get; set; }

        // No viene de la API: se arma acá
        public CategoriaAlerta Categoria => (Tipo, Subtipo) switch
        {
            ("Sanitaria", "ProximaAplicacion") => CategoriaAlerta.ProximaAplicacion,
            ("Sanitaria", "Vencimiento") => CategoriaAlerta.VencimientoSanitario,
            ("PlanAlimenticio", "Vencimiento") => CategoriaAlerta.VencimientoPlan,
            ("DesvioPeso", "PorAnimal") => CategoriaAlerta.DesvioPeso,
            _ => CategoriaAlerta.Desconocida
        };

        public string TextoTipo => Categoria switch
        {
            CategoriaAlerta.ProximaAplicacion => "Próxima aplicación",
            CategoriaAlerta.VencimientoSanitario => "Vencimiento sanitario",
            CategoriaAlerta.VencimientoPlan => "Vencimiento de plan",
            CategoriaAlerta.DesvioPeso => "Desvío de peso",
            _ => "Alerta"
        };

        public string TextoFecha => Fecha.ToString("dd/MM/yyyy");

        // El desvío de peso es de algo que ya pasó (último pesaje): se cuenta hacia atrás
        public string TextoDiasRestantes => EsDesvioPeso
            ? DiasRestantes switch
            {
                0 => "Hoy",
                -1 => "Ayer",
                _ => $"Hace {-DiasRestantes} días"
            }
            : DiasRestantes switch
            {
                0 => "Hoy",
                1 => "Mañana",
                _ => $"En {DiasRestantes} días"
            };

        public bool EsPlan => Categoria == CategoriaAlerta.VencimientoPlan;

        public string TextoCantidadAnimales => $"{CantidadAnimales ?? 0} animales en el rodeo";

        public bool EsDesvioPeso => Categoria == CategoriaAlerta.DesvioPeso;

        // En el desvío, la fecha ya está en el período evaluado (es la del último pesaje)
        public bool MostrarFecha => !EsDesvioPeso;

        public string TextoPeriodo =>
            $"Período evaluado: {PeriodoDesde?.ToString("dd/MM/yyyy") ?? "—"} al {PeriodoHasta?.ToString("dd/MM/yyyy") ?? "—"}";

        // Línea compacta del pantallazo: en el desvío, solo los números (la descripción del back no entra en una línea)
        public string TextoResumen => EsDesvioPeso
            ? string.Format(CulturaTexto, "{0:0.00} de {1:0.00} kg/día", GananciaReal ?? 0, GananciaEsperada ?? 0)
            : Descripcion;
    }
}
