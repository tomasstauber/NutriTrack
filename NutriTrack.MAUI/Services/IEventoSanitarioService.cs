using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public interface IEventoSanitarioService
    {
        // GET api/EventoSanitario/animal/{idAnimal}: historial sanitario de un animal,
        // del más reciente al más viejo. Sin eventos llega una lista vacía (no es un error).
        // 404 si no existe un animal con ese id.
        Task<ResultadoApi<List<EventoHistorial>>> ObtenerHistorialAnimalAsync(int idAnimal);

        // POST api/EventoSanitario/multiple: registra el evento en cada animal alcanzado.
        // No es atómico: un doble envío duplica los eventos
        Task<ResultadoApi<EventoSanitarioRegistrado>> RegistrarMultipleAsync(RegistrarEventoSanitarioRequest pedido);
    }
}
