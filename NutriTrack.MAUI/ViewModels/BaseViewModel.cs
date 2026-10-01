using CommunityToolkit.Mvvm.ComponentModel;

namespace NutriTrack.MAUI.ViewModels
{
    // Clase base de todos los ViewModels de NutriTrack osea para no escribir siempre los mismos mensajes cada vez
    //que hagamos una pantalla no repitas codigo enotonces cada viewmodel llama a esta base para tales mensajes
    // Centraliza el estado "cargando" y el mensaje de error que comparten las pantallas
    public partial class BaseViewModel : ObservableObject
    {
        // true mientras se espera una respuesta de la API
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NoEstaOcupado))]
        public partial bool EstaOcupado { get; set; }

        // Lo contrario de EstaOcupado, para habilitar botones desde el XAML
        public bool NoEstaOcupado => !EstaOcupado;

        // Mensaje de error para mostrar en pantalla (null si no hay error)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayError))]
        public partial string? MensajeError { get; set; }

        public bool HayError => !string.IsNullOrEmpty(MensajeError);

        // Ejecuta una operación mostrando "cargando" y limpiando errores anteriores
        // Uso en cualquier ViewModel:  await EjecutarAsync(async () => { ... });
        protected async Task EjecutarAsync(Func<Task> operacion)
        {
            if (EstaOcupado)
                return;

            try
            {
                EstaOcupado = true;
                MensajeError = null;
                await operacion();
            }
            finally
            {
                EstaOcupado = false;
            }
        }
    }
}