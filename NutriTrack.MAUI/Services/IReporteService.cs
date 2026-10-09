using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Reportes (CU17, CU18, CU19). Solo Administrador: al resto el back responde 403.
    // Los tres reciben el mismo filtro de período; cada reporte suma su método acá
    public interface IReporteService
    {
        // GET api/ReporteInventarioAnimales (CU17). Animales activos dados de alta en el período.
        // Sin resultados o con un rodeo inexistente responde 404 con mensaje
        Task<ResultadoApi<ReporteInventario>> InventarioAnimalesAsync(FiltroReporte filtro);

        // GET api/ReporteInventarioAnimales/pdf: el mismo reporte en PDF, con los mismos parámetros
        // y las mismas validaciones (404 / 400 en texto plano)
        Task<ResultadoApi<ArchivoDescargado>> InventarioAnimalesPdfAsync(FiltroReporte filtro);

        // GET api/ReporteFechasImportantes (CU18). Próximas aplicaciones y vencimientos con fecha en el período.
        // Con las dos listas vacías o con un rodeo inexistente responde 404 con mensaje
        Task<ResultadoApi<ReporteFechasImportantes>> FechasImportantesAsync(FiltroReporte filtro);

        // GET api/ReporteFechasImportantes/pdf: el mismo reporte en PDF, con los mismos parámetros
        // y las mismas validaciones (404 / 400 en texto plano)
        Task<ResultadoApi<ArchivoDescargado>> FechasImportantesPdfAsync(FiltroReporte filtro);

        // TODO #118: evolución de peso (GET api/ReporteEvolucionPeso), con el filtro de caravana
    }
}
