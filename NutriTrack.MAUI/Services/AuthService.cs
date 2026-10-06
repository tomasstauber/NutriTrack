using System.Net;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    public class AuthService : ApiServiceBase, IAuthService
    {
        private readonly ISesionService _sesionService;

        public AuthService(IHttpClientFactory httpClientFactory, ISesionService sesionService)
            : base(httpClientFactory)
        {
            _sesionService = sesionService;
        }

        public async Task<ResultadoApi<SesionUsuario>> IniciarSesionAsync(string nombreUsuario, string contrasenia)
        {
            // Si quedó una sesión anterior, el AuthHeaderHandler mandaría su token
            // y un 401 por contraseña incorrecta se tomaría como "sesión expirada"
            _sesionService.CerrarSesion();

            var pedido = new LoginRequest
            {
                NombreUsuario = nombreUsuario,
                Contrasenia = contrasenia
            };

            var resultado = await PostAsync<LoginResponse>("api/Login/login", pedido);

            if (!resultado.Exito)
            {
                // En el login, 401 significa credenciales inválidas, no sesión vencida
                if (resultado.CodigoEstado == HttpStatusCode.Unauthorized)
                    return ResultadoApi<SesionUsuario>.Error(
                        "Usuario o contraseña incorrectos.", resultado.CodigoEstado);

                // Sin conexión, timeout, 500...: el mensaje ya lo armó ApiServiceBase
                return ResultadoApi<SesionUsuario>.Error(
                    resultado.MensajeError ?? "Ocurrió un error inesperado.", resultado.CodigoEstado);
            }

            var respuesta = resultado.Datos;
            if (respuesta is null || string.IsNullOrEmpty(respuesta.Token))
                return ResultadoApi<SesionUsuario>.Error(
                    "La respuesta del servidor no tiene el formato esperado.");

            // Solo se aceptan los nombres exactos del enum (no números ni combinaciones)
            if (!Enum.GetNames<RolUsuario>().Contains(respuesta.Rol))
                return ResultadoApi<SesionUsuario>.Error(
                    "Tu usuario tiene un rol no reconocido. Contactá al administrador.");

            var rol = Enum.Parse<RolUsuario>(respuesta.Rol);

            var sesion = new SesionUsuario
            {
                Token = respuesta.Token,
                // Ya llega en UTC; ToUniversalTime asegura que se guarde como tal
                Expiracion = respuesta.Expiracion.ToUniversalTime(),
                NombreUsuario = respuesta.NombreUsuario,
                Rol = rol
            };

            await _sesionService.IniciarSesionAsync(sesion);
            return ResultadoApi<SesionUsuario>.Ok(sesion);
        }
    }
}
