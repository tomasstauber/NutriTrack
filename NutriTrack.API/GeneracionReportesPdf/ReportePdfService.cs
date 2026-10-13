using NutriTrack.API.DTOs;
using QuestPDF.Fluent;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class ReportePdfService : IReportePdfService
    {
        public byte[] GenerarInventario(ReporteInventarioAnimalesResponseDTO reporte, EncabezadoReporte encabezado)
            => new InventarioPdfDocument(reporte, encabezado).GeneratePdf();

        public byte[] GenerarFechasImportantes(ReporteFechasImportantesResponseDTO reporte, EncabezadoReporte encabezado)
            => new FechasImportantesPdfDocument(reporte, encabezado).GeneratePdf();

        public byte[] GenerarEvolucionPeso(ReporteEvolucionPesoResponseDTO reporte, EncabezadoReporte encabezado)
            => new EvolucionPesoPdfDocument(reporte, encabezado).GeneratePdf();
    }
}