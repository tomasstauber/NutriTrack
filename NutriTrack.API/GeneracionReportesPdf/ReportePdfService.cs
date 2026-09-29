using NutriTrack.API.DTOs;
using QuestPDF.Fluent;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public class ReportePdfService : IReportePdfService
    {
        public byte[] GenerarInventario(ReporteInventarioAnimalesResponseDTO reporte)
            => new InventarioPdfDocument(reporte).GeneratePdf();
    }
}