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
    }
}
