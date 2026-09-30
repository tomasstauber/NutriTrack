using NutriTrack.API.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class EvolucionPesoPdfDocument : IDocument
    {
        private readonly ReporteEvolucionPesoResponseDTO _reporte;

        public EvolucionPesoPdfDocument(ReporteEvolucionPesoResponseDTO reporte)
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

                page.Header().Text("Reporte de Evolución de Peso")
                    .FontSize(18).Bold();

                page.Content().PaddingVertical(10).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.RelativeColumn(2); // Caravana
                            c.RelativeColumn(2); // Fecha inicial
                            c.RelativeColumn(1); // Peso inicial
                            c.RelativeColumn(2); // Fecha final
                            c.RelativeColumn(1); // Peso final
                            c.RelativeColumn(1); // Variacion
                            c.RelativeColumn(1); // Cantidad registros
                        });

                        table.Header(h =>
                        {
                            foreach (var titulo in new[] { "Caravana", "Fecha Inicial", "Peso Inicial (kg)", "Fecha Final", "Peso Final (kg)", "Variación (kg)", "Registros" })
                                h.Cell().Background(Colors.Blue.Darken2).Padding(4)
                                 .Text(titulo).FontColor(Colors.White).Bold();
                        });

                        foreach (var a in _reporte.Animales)
                        {
                            table.Cell().Element(Celda).Text(a.Caravana);
                            table.Cell().Element(Celda).Text(a.FechaInicial.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Celda).Text(a.PesoInicial.ToString("0.0"));
                            table.Cell().Element(Celda).Text(a.FechaFinal.ToString("dd/MM/yyyy"));
                            table.Cell().Element(Celda).Text(a.PesoFinal.ToString("0.0"));
                            table.Cell().Element(Celda).Text(a.VariacionKg.HasValue ? a.VariacionKg.Value.ToString("0.0") : "—");
                            table.Cell().Element(Celda).Text(a.CantidadRegistros.ToString());
                        }
                    });

                    if (_reporte.Detalle != null)
                    {
                        col.Item().PaddingTop(20).Text("Detalle de pesajes").FontSize(13).Bold();

                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2); // Fecha
                                c.RelativeColumn(1); // Peso
                            });

                            table.Header(h =>
                            {
                                foreach (var titulo in new[] { "Fecha de Pesaje", "Peso (kg)" })
                                    h.Cell().Background(Colors.Blue.Lighten1).Padding(4)
                                     .Text(titulo).FontColor(Colors.White).Bold();
                            });

                            foreach (var p in _reporte.Detalle.Pesajes)
                            {
                                table.Cell().Element(Celda).Text(p.FechaPesaje.ToString("dd/MM/yyyy"));
                                table.Cell().Element(Celda).Text(p.PesoKg.ToString("0.0"));
                            }
                        });
                    }
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