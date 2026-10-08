using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IIngredienteService
    {
        // Lista los ingredientes activos del catálogo.
        Task<ResultadoApi<List<Ingrediente>>> ListarAsync();

        // POST api/Ingrediente: devuelve el ingrediente creado, con su id.
        Task<ResultadoApi<Ingrediente>> CrearAsync(IngredienteRequest ingrediente);
    }
}
