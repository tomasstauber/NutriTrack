using NutriTrack.API.DTOs;
using QuestPDF.Fluent;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class ReportePdfService : IReportePdfService
    {
        public byte[] GenerarInventario(ReporteInventarioAnimalesResponseDTO reporte)
            => new InventarioPdfDocument(reporte).GeneratePdf();

        public byte[] GenerarFechasImportantes(ReporteFechasImportantesResponseDTO reporte)
            => new FechasImportantesPdfDocument(reporte).GeneratePdf();

        public byte[] GenerarEvolucionPeso(ReporteEvolucionPesoResponseDTO reporte)
            => new EvolucionPesoPdfDocument(reporte).GeneratePdf();
    }
}