using NutriTrack.API.DTOs;

namespace NutriTrack.API.GeneracionReportesPdf
{
    public interface IReportePdfService
    {
        byte[] GenerarInventario(ReporteInventarioAnimalesResponseDTO reporte);
    }
}