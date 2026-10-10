namespace NutriTrack.MAUI.Models
{
    // Módulos en los que se agrupan las opciones del panel.
    // El orden del enum es el orden en pantalla.
    public enum GrupoModulo
    {
        Animal,
        Sanitario,
        Alimenticio,
        Administracion
    }

    // Una tarjeta de módulo del panel principal con las opciones que ve el rol
    public class GrupoMenu
    {
        // Nombre del enum como texto: el XAML elige el color del módulo con esto
        public string Clave { get; init; } = string.Empty;

        public string Titulo { get; init; } = string.Empty;

        public string Descripcion { get; init; } = string.Empty;

        // Dos letras que se muestran en el recuadro de la tarjeta (ej. "An")
        public string Sigla { get; init; } = string.Empty;

        public IReadOnlyList<EntradaMenu> Entradas { get; init; } = [];
    }
}
