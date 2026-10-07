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

        // Animal ACTIVO con esa caravana completa, sin distinguir mayúsculas, para conseguir
        // su id (la ficha no lo trae) y la caravana tal como está guardada.
        // Datos es null si no hay ninguno.
        Task<ResultadoApi<AnimalListado?>> BuscarActivoPorCaravanaAsync(string cuig, string nroManejo);

        // POST api/Animal: alta de un animal. Devuelve el id y la caravana del animal creado.
        // Los errores de negocio llegan como texto (409 caravana duplicada, 400 el resto).
        Task<ResultadoApi<AnimalCreado>> CrearAsync(CrearAnimalRequest animal);

        // PUT api/EdicionFichaAnimal: edita la ficha del animal (CU3). El 200 trae JSON y no se lee.
        // Los errores llegan como texto (404 si no existe, 400 el resto, incluida madre o padre inexistente).
        Task<ResultadoApi<bool>> EditarAsync(string cuig, string nroManejo, EditarAnimalRequest animal);

        // PATCH api/Animal/desactivar: da de baja al animal (CU4). El 200 trae texto plano y no se lee.
        // 400 "El animal ya está inactivo." si ya lo estaba; 404 si no existe.
        Task<ResultadoApi<bool>> DesactivarAsync(string cuig, string nroManejo);
    }
}
