namespace NutriTrack.MAUI.Models
{
    // Combinación de tipo y subtipo de una alerta. No viene de la API: la arma Alerta.
    // La pantalla elige el color con un DataTrigger según este valor
    public enum CategoriaAlerta
    {
        // Tipo o subtipo que el front todavía no conoce (ej. uno que se agregue en el back)
        Desconocida,
        ProximaAplicacion,
        VencimientoSanitario,
        VencimientoPlan
    }
}
