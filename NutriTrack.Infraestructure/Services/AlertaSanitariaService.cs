using NutriTrack.Core.Entities;
using NutriTrack.Infraestructure.Repositories;

namespace NutriTrack.Infraestructure.Services
{
    public class AlertaSanitariaService
    {
        private readonly EventoSanitarioRepository _eventoRepository;

        public AlertaSanitariaService(EventoSanitarioRepository eventoRepository)
        {
            _eventoRepository = eventoRepository;
        }

        public async Task<List<string>> VerificarAlertasAsync(int diasAviso = 7)
        {
            var hoy = DateTime.Today;
            var limite = hoy.AddDays(diasAviso);

            var eventos = await _eventoRepository
                .ObtenerEventosProximosAsync(hoy, limite);

            var alertas = new List<string>();

            foreach (var evento in eventos)
            {
                if (evento.FechaProximaAplicacion.HasValue &&
                    evento.FechaProximaAplicacion.Value.Date >= hoy &&
                    evento.FechaProximaAplicacion.Value.Date <= limite)
                {
                    alertas.Add(
                        $"Animal {evento.IdAnimal}: próxima aplicación " +
                        $"el {evento.FechaProximaAplicacion.Value:dd/MM/yyyy}. " +
                        $"Tipo de evento: {evento.TipoDeEvento}");
                }

                if (evento.VigenciaHasta.HasValue &&
                    evento.VigenciaHasta.Value.Date >= hoy &&
                    evento.VigenciaHasta.Value.Date <= limite)
                {
                    alertas.Add(
                        $"Animal {evento.IdAnimal}: vencimiento " +
                        $"el {evento.VigenciaHasta.Value:dd/MM/yyyy}. " +
                        $"Tipo de evento: {evento.TipoDeEvento}");
                }
            }

            return alertas;
        }
    }
}