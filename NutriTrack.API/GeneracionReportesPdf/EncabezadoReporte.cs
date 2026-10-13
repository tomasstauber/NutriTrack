using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace NutriTrack.API.GeneracionReportesPdf
{
    // Datos del encabezado de los PDF de reportes. No viaja en el JSON: solo lo usan los documentos
    public class EncabezadoReporte
    {
        // null en el inventario completo, que ignora las fechas
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }

        // null: sin filtro de rodeo
        public string? NombreRodeo { get; set; }

        // Solo en evolución de peso con caravana
        public string? Caravana { get; set; }

        // La pone el controller al generar el PDF: es la misma del nombre del archivo
        public DateTime Generado { get; set; }

        // Título y datos del reporte. Va en page.Header(), así se repite en cada página
        public void Componer(IContainer container, string titulo)
        {
            container.Column(col =>
            {
                col.Item().Text(titulo).FontSize(18).Bold();

                col.Item().PaddingTop(4).Text($"Período: {TextoPeriodo()}").FontSize(10);
                col.Item().Text($"Rodeo: {NombreRodeo ?? "Todos los rodeos"}").FontSize(10);

                if (!string.IsNullOrEmpty(Caravana))
                    col.Item().Text($"Caravana: {Caravana}").FontSize(10);

                col.Item().Text($"Generado: {Generado.ToString("dd/MM/yyyy HH:mm")}").FontSize(10);
            });
        }

        private string TextoPeriodo()
        {
            return Desde.HasValue && Hasta.HasValue
                ? $"Del {Desde.Value.ToString("dd/MM/yyyy")} al {Hasta.Value.ToString("dd/MM/yyyy")}"
                : $"Stock activo al {Generado.ToString("dd/MM/yyyy")}";
        }
    }
}
