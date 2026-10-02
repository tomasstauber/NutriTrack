using System.Net;
using System.Net.Http.Headers;

namespace NutriTrack.MAUI.Services
{
    // Se ejecuta en TODOS los pedidos del cliente "NutriTrackApi":
    // 1) Si hay sesion, agrega el token en el encabezado Authorization
    // 2) Si la API responde 401 teniendo sesion, avisa que la sesión expiro
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly ISesionService _sesionService;

        public AuthHeaderHandler(ISesionService sesionService)
        {
            _sesionService = sesionService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sesion = _sesionService.SesionActual;

            if (sesion is not null)
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", sesion.Token);
            }

            // Ac el pedido sale realmente hacia la API y esperamos la respuesta.
            var respuesta = await base.SendAsync(request, cancellationToken);

            if (respuesta.StatusCode == HttpStatusCode.Unauthorized && sesion is not null)
            {
                _sesionService.NotificarSesionExpirada();
            }

            return respuesta;
        }
    }
}