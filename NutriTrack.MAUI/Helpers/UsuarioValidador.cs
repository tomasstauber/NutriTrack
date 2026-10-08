using System.Net.Mail;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.Helpers
{
    // Validaciones de los datos de un usuario, compartidas por el alta y la edición.
    // Cada función devuelve el mensaje de error del campo, o null si está bien
    public static class UsuarioValidador
    {
        // R4: largo mínimo de la contraseña (igual que LimitesUsuario.ContraseniaMin de la API)
        public const int LargoMinimoContrasenia = 8;

        // Recibe el nombre ya recortado
        public static string? ValidarNombre(string nombre) =>
            string.IsNullOrEmpty(nombre) ? "El nombre completo es obligatorio." : null;

        // Recibe el correo ya recortado
        public static string? ValidarCorreo(string correo) =>
            string.IsNullOrEmpty(correo)
                ? "El correo electrónico es obligatorio."
                : !CorreoValido(correo) ? "El formato del correo electrónico no es válido." : null;

        // Recibe el nombre de usuario ya recortado
        public static string? ValidarNombreUsuario(string nombreUsuario) =>
            string.IsNullOrEmpty(nombreUsuario) ? "El nombre de usuario es obligatorio." : null;

        public static string? ValidarContrasenia(string contrasenia) =>
            contrasenia.Length < LargoMinimoContrasenia
                ? $"La contraseña debe tener al menos {LargoMinimoContrasenia} caracteres."
                : null;

        public static string? ValidarConfirmacionContrasenia(string contrasenia, string? confirmacion) =>
            contrasenia != (confirmacion ?? string.Empty) ? "Las contraseñas no coinciden." : null;

        public static string? ValidarRol(RolUsuario? rol) =>
            rol is null ? "Debe seleccionar un rol." : null;

        // R2: mismo criterio que la API (ValidarFormatoCorreo en UsuarioController)
        private static bool CorreoValido(string correo) =>
            MailAddress.TryCreate(correo, out var direccion) && direccion.Address == correo;
    }
}
