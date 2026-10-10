namespace NutriTrack.MAUI.Models
{
    // Una opción del menú del panel principal.
    // Cada módulo declara la suya en Helpers/MenuModulos.cs.
    public class EntradaMenu
    {
        public string Titulo { get; init; } = string.Empty;

        // Ruta de Shell de la pantalla principal del módulo (ej. "animales")
        public string Ruta { get; init; } = string.Empty;

        // Roles que ven esta opción en el menú
        public IReadOnlyList<RolUsuario> Roles { get; init; } = [];

        // Posición en el menú (de menor a mayor)
        public int Orden { get; init; }

        // Módulo en el que se agrupa la opción en el panel
        public GrupoModulo Grupo { get; init; }
    }
}
