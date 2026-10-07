namespace NutriTrack.MAUI.Models
{
    // Opción del Picker de filtro por rol. Rol null significa "Todos".
    public class OpcionRol
    {
        public string Texto { get; init; } = string.Empty;
        public RolUsuario? Rol { get; init; }
    }
}
