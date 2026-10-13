using NutriTrack.API.DTOs;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public interface IReportePdfService
    {
        byte[] GenerarInventario(ReporteInventarioAnimalesResponseDTO reporte, EncabezadoReporte encabezado);
        byte[] GenerarFechasImportantes(ReporteFechasImportantesResponseDTO reporte, EncabezadoReporte encabezado);
        byte[] GenerarEvolucionPeso(ReporteEvolucionPesoResponseDTO reporte, EncabezadoReporte encabezado);
    }
}