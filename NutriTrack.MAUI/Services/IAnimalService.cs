using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IAnimalService
    {
        // GET api/Animal: listado paginado de animales.
        // incluirInactivos = true suma también los animales dados de baja.
        Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            int pagina = 1, int tamanioPagina = 50, bool incluirInactivos = false);
    }
}
