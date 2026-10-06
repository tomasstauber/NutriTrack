using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IIngredienteService
    {
        // Lista los ingredientes activos del catálogo.
        Task<ResultadoApi<List<Ingrediente>>> ListarAsync();
    }
}
