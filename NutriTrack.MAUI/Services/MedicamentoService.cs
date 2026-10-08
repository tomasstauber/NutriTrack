using System.Net;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class MedicamentoService : ApiServiceBase, IMedicamentoService
    {
        public MedicamentoService(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<ResultadoApi<List<Medicamento>>> ListarAsync(string? nombre = null, bool incluirInactivos = false)
        {
            var parametros = new List<string>();

            var textoBusqueda = nombre?.Trim();
            if (!string.IsNullOrEmpty(textoBusqueda))
                parametros.Add($"nombre={Uri.EscapeDataString(textoBusqueda)}");

            if (incluirInactivos)
                parametros.Add("incluirInactivos=true");

            var url = parametros.Count == 0
                ? "api/Medicamento"
                : $"api/Medicamento?{string.Join("&", parametros)}";

            var resultado = await GetAsync<List<Medicamento>>(url);

            // R1: sin incluirInactivos solo se devuelven los activos. Falta confirmar
            // que el back no los mande, así que se filtra también acá
            if (!resultado.Exito || incluirInactivos)
                return resultado;

            var activos = (resultado.Datos ?? []).Where(m => m.Activo).ToList();
            return ResultadoApi<List<Medicamento>>.Ok(activos, resultado.CodigoEstado ?? HttpStatusCode.OK);
        }

        public Task<ResultadoApi<Medicamento>> CrearAsync(MedicamentoRequest medicamento) =>
            PostAsync<Medicamento>("api/Medicamento", medicamento);

        public Task<ResultadoApi<bool>> EditarAsync(int idMedicamento, MedicamentoRequest medicamento) =>
            PutSinRespuestaAsync($"api/Medicamento/{idMedicamento}", medicamento);

        public Task<ResultadoApi<bool>> DesactivarAsync(int idMedicamento) =>
            DeleteAsync($"api/Medicamento/{idMedicamento}");

        public Task<ResultadoApi<bool>> ReactivarAsync(int idMedicamento) =>
            PatchSinRespuestaAsync($"api/Medicamento/activar/{idMedicamento}");
    }
}
