using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Clase base para todos los servicios que hablan con la API
    // (AuthService, AnimalService, y demas)
    // Es comoo la centrla del manejo de errores para que ninguna pantalla tenga que hacerlo
    public abstract class ApiServiceBase
    {
        protected readonly HttpClient Http;

        // Mismas reglas de JSON que la API: camelCase y enums como texto.
        protected static readonly JsonSerializerOptions OpcionesJson =
            new(JsonSerializerDefaults.Web)
            {
                Converters = { new JsonStringEnumConverter() }
            };

        protected ApiServiceBase(IHttpClientFactory httpClientFactory)
        {
            Http = httpClientFactory.CreateClient("NutriTrackApi");
        }

        //Pedidos que devuelven datos 
        protected Task<ResultadoApi<T>> GetAsync<T>(string url) =>
            EnviarAsync<T>(() => Http.GetAsync(url), leerDatos: true);

        protected Task<ResultadoApi<T>> PostAsync<T>(string url, object cuerpo) =>
            EnviarAsync<T>(() => Http.PostAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: true);

        protected Task<ResultadoApi<T>> PutAsync<T>(string url, object cuerpo) =>
            EnviarAsync<T>(() => Http.PutAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: true);

        //Pedidos donde solo importa si saliio t odo  bien 

        protected Task<ResultadoApi<bool>> PostSinRespuestaAsync(string url, object cuerpo) =>
            EnviarAsync<bool>(() => Http.PostAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: false);

        protected Task<ResultadoApi<bool>> PutSinRespuestaAsync(string url, object cuerpo) =>
            EnviarAsync<bool>(() => Http.PutAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: false);

        protected Task<ResultadoApi<bool>> DeleteAsync(string url) =>
            EnviarAsync<bool>(() => Http.DeleteAsync(url), leerDatos: false);

        // DELETE con cuerpo JSON (ej. EliminarRodeo): HttpClient.DeleteAsync no acepta cuerpo
        protected Task<ResultadoApi<bool>> DeleteAsync(string url, object cuerpo) =>
            EnviarAsync<bool>(() => EnviarConCuerpoAsync(HttpMethod.Delete, url, cuerpo), leerDatos: false);

        //PATCH que devuelve datos (con T = string lee el texto plano del back)

        protected Task<ResultadoApi<T>> PatchAsync<T>(string url) =>
            EnviarAsync<T>(() => Http.PatchAsync(url, null), leerDatos: true);

        protected Task<ResultadoApi<T>> PatchAsync<T>(string url, object cuerpo) =>
            EnviarAsync<T>(() => Http.PatchAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: true);

        //PATCH donde solo importa si salio todo bien (no se lee la respuesta)

        protected Task<ResultadoApi<bool>> PatchSinRespuestaAsync(string url) =>
            EnviarAsync<bool>(() => Http.PatchAsync(url, null), leerDatos: false);

        protected Task<ResultadoApi<bool>> PatchSinRespuestaAsync(string url, object cuerpo) =>
            EnviarAsync<bool>(() => Http.PatchAsJsonAsync(url, cuerpo, OpcionesJson), leerDatos: false);

        // Arma el pedido a mano para los verbos que HttpClient no deja mandar con cuerpo
        private async Task<HttpResponseMessage> EnviarConCuerpoAsync(HttpMethod metodo, string url, object cuerpo)
        {
            using var pedido = new HttpRequestMessage(metodo, url)
            {
                Content = JsonContent.Create(cuerpo, cuerpo.GetType(), options: OpcionesJson)
            };
            return await Http.SendAsync(pedido);
        }

        //Es uun solo lugar que maneja todos los casos 

        private async Task<ResultadoApi<T>> EnviarAsync<T>(
            Func<Task<HttpResponseMessage>> pedido, bool leerDatos)
        {
            try
            {
                using var respuesta = await pedido();

                if (!respuesta.IsSuccessStatusCode)
                {
                    var mensaje = await LeerMensajeErrorAsync(respuesta);
                    return ResultadoApi<T>.Error(mensaje, respuesta.StatusCode);
                }

                if (!leerDatos)
                    return ResultadoApi<T>.Ok(default, respuesta.StatusCode);

                // Algunos endpoints devuelven un texto plano en vez de JSON
                if (typeof(T) == typeof(string))
                {
                    var texto = await respuesta.Content.ReadAsStringAsync();
                    return ResultadoApi<T>.Ok((T)(object)texto, respuesta.StatusCode);
                }

                var datos = await respuesta.Content.ReadFromJsonAsync<T>(OpcionesJson);
                return ResultadoApi<T>.Ok(datos, respuesta.StatusCode);
            }
            catch (TaskCanceledException)
            {
                return ResultadoApi<T>.Error(
                    "El servidor tardó demasiado en responder. Intentá de nuevo.");
            }
            catch (HttpRequestException)
            {
                return ResultadoApi<T>.Error(
                    "No se pudo conectar con el servidor. Verificá tu conexión a internet.");
            }
            catch (JsonException)
            {
                return ResultadoApi<T>.Error(
                    "La respuesta del servidor no tiene el formato esperado.");
            }
        }

        // Convierte la respuesta de error de la API en un mensaje para el usuario
        private static async Task<string> LeerMensajeErrorAsync(HttpResponseMessage respuesta)
        {
            // Errores 500: nunca mostramos el detalle interno del servidor
            if ((int)respuesta.StatusCode >= 500)
                return MensajePorDefecto(respuesta.StatusCode);

            // 403: el back hace Forbid() sin cuerpo; el mensaje es siempre el mismo
            if (respuesta.StatusCode == HttpStatusCode.Forbidden)
                return MensajePorDefecto(respuesta.StatusCode);

            var contenido = (await respuesta.Content.ReadAsStringAsync()).Trim();

            if (string.IsNullOrEmpty(contenido))
                return MensajePorDefecto(respuesta.StatusCode);

            // Caso 1: errores de validación automáticos de ASP.NET (JSON con "errors")
            if (contenido.StartsWith('{'))
            {
                try
                {
                    using var documento = JsonDocument.Parse(contenido);
                    if (documento.RootElement.TryGetProperty("errors", out var errores))
                    {
                        foreach (var campo in errores.EnumerateObject())
                            foreach (var detalle in campo.Value.EnumerateArray())
                                return detalle.GetString() ?? MensajePorDefecto(respuesta.StatusCode);
                    }
                }
                catch (JsonException) { }

                return MensajePorDefecto(respuesta.StatusCode);
            }

            // Caso 2: mensajes de texto de los controllers, ej. BadRequest("La caravana ya existe")
            return contenido.Trim('"');
        }

        private static string MensajePorDefecto(HttpStatusCode codigo) => codigo switch
        {
            HttpStatusCode.BadRequest => "Los datos enviados no son válidos.",
            HttpStatusCode.Unauthorized => "Tu sesión expiró. Volvé a iniciar sesión.",
            HttpStatusCode.Forbidden => "Tu rol no tiene permiso para esta acción",
            HttpStatusCode.NotFound => "No se encontró lo que buscabas.",
            HttpStatusCode.Conflict => "La operación no se puede realizar en el estado actual.",
            _ when (int)codigo >= 500 => "Ocurrió un error en el servidor. Intentá de nuevo más tarde.",
            _ => "Ocurrió un error inesperado."
        };
    }
}