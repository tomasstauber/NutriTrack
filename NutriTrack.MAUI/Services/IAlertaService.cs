using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Alertas vigentes (CU15 sanitarias y CU16 planes). Solo Administrador: al resto el back responde 403
    public interface IAlertaService
    {
        // GET api/Alerta. Sin alertas responde 200 con la lista vacía
        Task<ResultadoApi<AlertasVigentes>> ListarAsync();
    }
}
