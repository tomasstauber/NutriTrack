using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IIngredienteService
    {
        // Lista los ingredientes activos del catálogo.
        Task<ResultadoApi<List<Ingrediente>>> ListarAsync();

        // POST api/Ingrediente: devuelve el ingrediente creado, con su id.
        Task<ResultadoApi<Ingrediente>> CrearAsync(IngredienteRequest ingrediente);

        // PUT api/Ingrediente/{id}: el back responde un texto, no se lee.
        Task<ResultadoApi<bool>> ActualizarAsync(int id, IngredienteRequest ingrediente);

        // DELETE api/Ingrediente/{id} (baja lógica): devuelve el texto del back,
        // que avisa en qué planes se usaba el ingrediente.
        Task<ResultadoApi<string>> DesactivarAsync(int id);
    }
}
