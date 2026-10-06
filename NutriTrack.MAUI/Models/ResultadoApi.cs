using System.Net;

namespace NutriTrack.MAUI.Models
{
    // Resultado de cualquier pedido a la API
    // Si Exito es true, los datos están en Datos
    // Si es false, MensajeError trae un texto listo para mostrarle  al usuario
    public class ResultadoApi<T>
    {
        public bool Exito { get; private init; }
        public T? Datos { get; private init; }
        public string? MensajeError { get; private init; }
        public HttpStatusCode? CodigoEstado { get; private init; }

        public static ResultadoApi<T> Ok(T? datos) =>
            new() { Exito = true, Datos = datos };

        // Igual que Ok, pero guarda el código HTTP de la respuesta (200, 204...)
        public static ResultadoApi<T> Ok(T? datos, HttpStatusCode codigo) =>
            new() { Exito = true, Datos = datos, CodigoEstado = codigo };

        public static ResultadoApi<T> Error(string mensaje, HttpStatusCode? codigo = null) =>
            new() { Exito = false, MensajeError = mensaje, CodigoEstado = codigo };
    }
}