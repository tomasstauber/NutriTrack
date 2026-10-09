using System.Globalization;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class ReporteService : ApiServiceBase, IReporteService
    {
        public ReporteService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public Task<ResultadoApi<ReporteInventario>> InventarioAnimalesAsync(FiltroReporte filtro) =>
            GetAsync<ReporteInventario>($"api/ReporteInventarioAnimales?{ArmarQuery(filtro)}");

        public Task<ResultadoApi<ArchivoDescargado>> InventarioAnimalesPdfAsync(FiltroReporte filtro) =>
            GetArchivoAsync($"api/ReporteInventarioAnimales/pdf?{ArmarQuery(filtro)}");

        public Task<ResultadoApi<ReporteFechasImportantes>> FechasImportantesAsync(FiltroReporte filtro) =>
            GetAsync<ReporteFechasImportantes>($"api/ReporteFechasImportantes?{ArmarQuery(filtro)}");

        public Task<ResultadoApi<ArchivoDescargado>> FechasImportantesPdfAsync(FiltroReporte filtro) =>
            GetArchivoAsync($"api/ReporteFechasImportantes/pdf?{ArmarQuery(filtro)}");

        // Parámetros comunes a los tres reportes. Solo se mandan los que tienen valor:
        // FiltroReporte ya viene con lo que corresponde al tipo elegido
        private static string ArmarQuery(FiltroReporte filtro)
        {
            var parametros = new List<string>
            {
                $"tipoReporte={Uri.EscapeDataString(filtro.TipoReporte)}"
            };

            if (filtro.IdRodeo.HasValue)
                parametros.Add($"idRodeo={filtro.IdRodeo.Value}");

            if (filtro.FechaReferencia.HasValue)
                parametros.Add($"fechaReferencia={FormatearFecha(filtro.FechaReferencia.Value)}");

            if (filtro.Desde.HasValue)
                parametros.Add($"desde={FormatearFecha(filtro.Desde.Value)}");

            if (filtro.Hasta.HasValue)
                parametros.Add($"hasta={FormatearFecha(filtro.Hasta.Value)}");

            return string.Join("&", parametros);
        }

        // Siempre ISO solo fecha ("2026-10-05"): el back lee "5/10" como 10 de mayo, sin avisar
        private static string FormatearFecha(DateTime fecha) =>
            Uri.EscapeDataString(fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
