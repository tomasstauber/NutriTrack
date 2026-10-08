using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IPlanAlimenticioService
    {
        // GET api/PlanAlimenticio: lista de planes.
        // Solo Asesor técnico y Administrador: al resto el back responde 403.
        Task<ResultadoApi<List<PlanAlimenticio>>> ListarAsync();

        // POST api/PlanAlimenticio: crea el plan con sus componentes (CU9).
        // El back responde un texto de confirmación; los errores llegan en texto (400),
        // por ejemplo "Ya existe un plan con ese nombre."
        Task<ResultadoApi<bool>> CrearAsync(PlanAlimenticioRequest plan);

        // GET api/PlanAlimenticio/{id}: plan con sus componentes y las asignaciones vigentes (CU11).
        // 404 si no existe
        Task<ResultadoApi<PlanAlimenticioCompleto>> ObtenerAsync(int id);

        // PUT api/PlanAlimenticio/{id}: edita el plan con el mismo cuerpo del alta (CU11).
        // El back responde un texto, no se lee. Errores en texto (404, 400),
        // por ejemplo "Ya existe otro plan con ese nombre"
        Task<ResultadoApi<bool>> ActualizarAsync(int id, PlanAlimenticioRequest plan);

        // POST api/PlanRodeoAsignacion: asigna el plan a un rodeo (CU10).
        // Si el rodeo tenía otro plan activo, el back lo cierra y lo informa en la respuesta.
        // Errores en texto (404 plan o rodeo, 400 el resto). Solo Asesor técnico y Administrador
        Task<ResultadoApi<AsignarPlanResponse>> AsignarARodeoAsync(AsignarPlanRequest asignacion);
    }
}
