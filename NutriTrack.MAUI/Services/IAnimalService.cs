using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IAnimalService
    {
        // GET api/Animal: listado paginado de animales.
        // incluirInactivos = true suma también los animales dados de baja.
        Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            int pagina = 1, int tamanioPagina = 50, bool incluirInactivos = false);

        // GET api/Animal con todos los filtros.
        // texto busca por caravana o raza; vacío o null trae todo.
        // texto no tiene valor por defecto para no chocar con la sobrecarga de arriba.
        Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            string? texto, int? idRodeo = null, bool sinRodeo = false,
            bool incluirInactivos = false, int pagina = 1, int tamanioPagina = 50);

        // GET api/ConsultaFichaIndividualAnimal: ficha de un animal por su caravana.
        Task<ResultadoApi<FichaAnimal>> ObtenerFichaAsync(string cuig, string nroManejo);
    }
}
