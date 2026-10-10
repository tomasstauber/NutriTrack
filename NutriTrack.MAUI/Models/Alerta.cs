namespace NutriTrack.MAUI.Models
{
    // Alerta vigente de GET api/Alerta (en el back: AlertaItemDTO)
    public class Alerta
    {
        // "Sanitaria" / "PlanAlimenticio". Texto y no enum: un tipo nuevo del back
        // no tiene que romper la lectura de toda la lista
        public string Tipo { get; set; } = string.Empty;

        // "ProximaAplicacion" / "Vencimiento"
        public string Subtipo { get; set; } = string.Empty;

        // Caravana "CUIG-NRO" en las sanitarias; nombre del rodeo en las de plan
        public string Destino { get; set; } = string.Empty;

        // Medicamentos o tipo de evento en las sanitarias; nombre del plan en las de plan
        public string Descripcion { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; }

        // 0 = hoy
        public int DiasRestantes { get; set; }

        // Solo planes; null en las sanitarias
        public DateOnly? VigenciaDesde { get; set; }
        public int? CantidadAnimales { get; set; }

        // No viene de la API: se arma acá
        public CategoriaAlerta Categoria => (Tipo, Subtipo) switch
        {
            ("Sanitaria", "ProximaAplicacion") => CategoriaAlerta.ProximaAplicacion,
            ("Sanitaria", "Vencimiento") => CategoriaAlerta.VencimientoSanitario,
            ("PlanAlimenticio", "Vencimiento") => CategoriaAlerta.VencimientoPlan,
            _ => CategoriaAlerta.Desconocida
        };

        public string TextoTipo => Categoria switch
        {
            CategoriaAlerta.ProximaAplicacion => "Próxima aplicación",
            CategoriaAlerta.VencimientoSanitario => "Vencimiento sanitario",
            CategoriaAlerta.VencimientoPlan => "Vencimiento de plan",
            _ => "Alerta"
        };

        public string TextoFecha => Fecha.ToString("dd/MM/yyyy");

        public string TextoDiasRestantes => DiasRestantes switch
        {
            0 => "Hoy",
            1 => "Mañana",
            _ => $"En {DiasRestantes} días"
        };

        public bool EsPlan => Categoria == CategoriaAlerta.VencimientoPlan;

        public string TextoCantidadAnimales => $"{CantidadAnimales ?? 0} animales en el rodeo";
    }
}
