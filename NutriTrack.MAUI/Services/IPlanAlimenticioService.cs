using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IPlanAlimenticioService
    {
        // GET api/PlanAlimenticio: lista de planes.
        // Solo Asesor técnico y Administrador: al resto el back responde 403.
        Task<ResultadoApi<List<PlanAlimenticio>>> ListarAsync();
    }
}
