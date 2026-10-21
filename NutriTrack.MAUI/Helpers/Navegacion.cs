using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace NutriTrack.MAUI.Helpers
{
    // Comando del botón "Volver" que agrega la plantilla PaginaConVolver (Styles.xaml)
    // a todas las páginas. Hace lo mismo que la flecha de la barra del Shell.
    // Es AsyncRelayCommand: mientras navega queda deshabilitado, así un doble toque
    // no retrocede dos pantallas
    public static class Navegacion
    {
        public static ICommand VolverCommand { get; } =
            new AsyncRelayCommand(() => Shell.Current.GoToAsync(".."));
    }
}
