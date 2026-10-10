using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Services
{
    // Alertas marcadas como leídas, guardadas en el dispositivo y por usuario de la sesión.
    // Las alertas se calculan al consultar y no traen id: cada una se identifica con una clave
    // armada con tipo, subtipo, destino y fecha
    public interface IAlertasLeidasService
    {
        // Único lugar donde se arma la clave de una alerta
        string Clave(Alerta alerta);

        // Claves leídas del usuario de la sesión. Borra del guardado las que ya no llegan del back.
        // Llamar solo con una respuesta exitosa: sin respuesta no se sabe qué alertas existen.
        // Si el guardado falla o está corrupto devuelve vacío (todo no leído)
        IReadOnlySet<string> ObtenerLeidas(IEnumerable<Alerta> vigentes);

        // Reemplaza las claves leídas del usuario de la sesión. Si falla, no se guarda y no rompe la pantalla
        void GuardarLeidas(IEnumerable<string> claves);
    }
}
