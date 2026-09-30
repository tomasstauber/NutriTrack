using NutriTrack.API.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class FechasImportantesPdfDocument : IDocument
    {
        private readonly ReporteFechasImportantesResponseDTO _reporte;

        public FechasImportantesPdfDocument(ReporteFechasImportantesResponseDTO reporte)
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

                page.Header().Text("Reporte de Fechas Importantes a Recordar")
                    .FontSize(18).Bold();

                page.Content().PaddingVertical(10).Column(col =>
                {
                    
                    col.Item().PaddingTop(5).Text("Próximas aplicaciones sanitarias")
                        .FontSize(13).Bold();

                    if (_reporte.ProximasAplicaciones.Count == 0)
                    {
                        col.Item().PaddingVertical(5).Text("No hay aplicaciones próximas en el período.").Italic();
                    }
                    else
                    {
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2); // Destino
                                c.RelativeColumn(2); // Tipo evento
                                c.RelativeColumn(2); // Producto
                                c.RelativeColumn(2); // Fecha
                                c.RelativeColumn(2); // Responsable
                            });

                            table.Header(h =>
                            {
                                foreach (var titulo in new[] { "Destino", "Tipo Evento", "Producto", "Fecha Próx. Aplicación", "Responsable" })
                                    h.Cell().Background(Colors.Green.Darken2).Padding(4)
                                     .Text(titulo).FontColor(Colors.White).Bold();
                            });

                            foreach (var p in _reporte.ProximasAplicaciones)
                            {
                                table.Cell().Element(Celda).Text(p.Destino);
                                table.Cell().Element(Celda).Text(p.TipoEvento);
                                table.Cell().Element(Celda).Text(p.Producto);
                                table.Cell().Element(Celda).Text(p.FechaProximaAplicacion.ToString("dd/MM/yyyy"));
                                table.Cell().Element(Celda).Text(p.Responsable);
                            }
                        });
                    }


                    col.Item().PaddingTop(20).Text("Vencimientos de eventos sanitarios")
                        .FontSize(13).Bold();

                    if (_reporte.VencimientosEventos.Count == 0)
                    {
                        col.Item().PaddingVertical(5).Text("No hay vencimientos en el período.").Italic();
                    }
                    else
                    {
                        col.Item().PaddingTop(5).Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(2); // Destino
                                c.RelativeColumn(2); // Tipo evento
                                c.RelativeColumn(2); // Producto
                                c.RelativeColumn(2); // Vigencia hasta
                                c.RelativeColumn(2); // Responsable
                                c.RelativeColumn(2); // Observaciones
                            });

                            table.Header(h =>
                            {
                                foreach (var titulo in new[] { "Destino", "Tipo Evento", "Producto", "Vigencia Hasta", "Responsable", "Observaciones" })
                                    h.Cell().Background(Colors.Orange.Darken2).Padding(4)
                                     .Text(titulo).FontColor(Colors.White).Bold();
                            });

                            foreach (var v in _reporte.VencimientosEventos)
                            {
                                table.Cell().Element(Celda).Text(v.Destino);
                                table.Cell().Element(Celda).Text(v.TipoEvento);
                                table.Cell().Element(Celda).Text(v.Producto);
                                table.Cell().Element(Celda).Text(v.VigenciaHasta.ToString("dd/MM/yyyy"));
                                table.Cell().Element(Celda).Text(v.Responsable);
                                table.Cell().Element(Celda).Text(v.Observaciones ?? "-");
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