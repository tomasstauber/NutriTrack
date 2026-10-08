using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IRodeoService
    {
        // Lista los rodeos activos, ordenados por nombre.
        Task<ResultadoApi<List<Rodeo>>> ListarAsync();

        // Datos para confirmar la eliminación: nombre, animales y planes asignados.
        Task<ResultadoApi<ResumenEliminacionRodeo>> ObtenerResumenEliminacionAsync(int idRodeo);

        // Elimina el rodeo (baja lógica); sus animales quedan sin rodeo.
        Task<ResultadoApi<bool>> EliminarAsync(int idRodeo);

        // Crea el rodeo con los animales indicados (deben estar activos y sin rodeo).
        Task<ResultadoApi<RodeoResponse>> CrearAsync(CrearRodeoRequest rodeo);

        // PATCH api/TransferenciaAnimal: pasa los animales del rodeo origen al destino (CU7).
        // Atómica: si un animal falla, no se transfiere ninguno. Errores en texto (404 rodeo, 400 el resto).
        Task<ResultadoApi<TransferenciaResponse>> TransferirAsync(TransferirAnimalesRequest transferencia);
    }
}
