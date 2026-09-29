using NutriTrack.API.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class InventarioPdfDocument : IDocument
    {
        private readonly ReporteInventarioAnimalesResponseDTO _reporte;

        public InventarioPdfDocument(ReporteInventarioAnimalesResponseDTO reporte)
        {
            _reporte = reporte;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A4.Landscape());

                page.Header().Text("Reporte de Inventario de Animales")
                    .FontSize(18).Bold();

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Text($"Total de animales: {_reporte.TotalAnimales}").FontSize(12);

                    // El header de la tabla se repite solo en cada pagina si el
                    // inventario tiene muchos animales 
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(2); // Caravana
                            c.RelativeColumn(2); // Raza
                            c.RelativeColumn(1); // Sexo
                            c.RelativeColumn(2); // Edad
                            c.RelativeColumn(2); // Fecha alta
                            c.RelativeColumn(1); // Estado
                            c.RelativeColumn(2); // Rodeo
                        });

                        table.Header(h =>
                        {
                            foreach (var titulo in new[] { "Caravana", "Raza", "Sexo", "Edad", "Fecha Alta", "Estado", "Rodeo" })
                                h.Cell().Background(Colors.Green.Darken2).Padding(4)
                                 .Text(titulo).FontColor(Colors.White).Bold();
                        });

                        foreach (var a in _reporte.Animales)
                        {
                            table.Cell().Element(Celda).Text(a.Caravana);
                            table.Cell().Element(Celda).Text(a.Raza);
                            table.Cell().Element(Celda).Text(a.Sexo);
                            table.Cell().Element(Celda).Text(a.Edad);
                            table.Cell().Element(Celda).Text(a.FechaAltaSistema.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Celda).Text(a.EstadoActual);
                            table.Cell().Element(Celda).Text(a.RodeoActual ?? "-");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Página ");
                    t.CurrentPageNumber();
                    t.Span(" de ");
                    t.TotalPages();
                });
            });
        }

        private static IContainer Celda(IContainer c) =>
            c.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4);
    }
}