using System.Net;
using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU21 - Crear usuario
    // Se llega desde "Nuevo usuario" de la lista de usuarios (solo Administrador)
    public partial class NuevoUsuarioViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "nuevo-usuario";

        // R4: largo mínimo de la contraseña (igual que LimitesUsuario.ContraseniaMin de la API)
        private const int LargoMinimoContrasenia = 8;

        private const string MensajeErrorCreacion = "No se pudo crear el usuario; intente nuevamente.";

        private readonly IUsuarioService _usuarioService;

        public NuevoUsuarioViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        // Los tres roles con su texto legible
        public IReadOnlyList<OpcionRol> OpcionesRol { get; } =
        [
            new() { Texto = "Administrador", Rol = RolUsuario.Administrador },
            new() { Texto = "Encargado de campo", Rol = RolUsuario.EncargadoDeCampo },
            new() { Texto = "Asesor técnico", Rol = RolUsuario.AsesorTecnico }
        ];

        // Datos del usuario

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Correo { get; set; }

        [ObservableProperty]
        public partial string? NombreUsuario { get; set; }

        [ObservableProperty]
        public partial string? Contrasenia { get; set; }

        // Solo se usa en la app para evitar errores de tipeo: no viaja a la API
        [ObservableProperty]
        public partial string? ConfirmacionContrasenia { get; set; }

        [ObservableProperty]
        public partial OpcionRol? RolSeleccionado { get; set; }

        // E1: error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorNombre))]
        public partial string? ErrorNombre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorCorreo))]
        public partial string? ErrorCorreo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorNombreUsuario))]
        public partial string? ErrorNombreUsuario { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorContrasenia))]
        public partial string? ErrorContrasenia { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorConfirmacionContrasenia))]
        public partial string? ErrorConfirmacionContrasenia { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorRol))]
        public partial string? ErrorRol { get; set; }

        public bool HayErrorNombre => !string.IsNullOrEmpty(ErrorNombre);
        public bool HayErrorCorreo => !string.IsNullOrEmpty(ErrorCorreo);
        public bool HayErrorNombreUsuario => !string.IsNullOrEmpty(ErrorNombreUsuario);
        public bool HayErrorContrasenia => !string.IsNullOrEmpty(ErrorContrasenia);
        public bool HayErrorConfirmacionContrasenia => !string.IsNullOrEmpty(ErrorConfirmacionContrasenia);
        public bool HayErrorRol => !string.IsNullOrEmpty(ErrorRol);

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            var nombre = Nombre?.Trim() ?? string.Empty;
            var correo = Correo?.Trim() ?? string.Empty;
            var nombreUsuario = NombreUsuario?.Trim() ?? string.Empty;
            // La contraseña no se recorta: los espacios son parte de ella
            var contrasenia = Contrasenia ?? string.Empty;
            var rol = RolSeleccionado?.Rol;

            // Se validan todos los campos a la vez, para marcar cada uno con su error
            ErrorNombre = string.IsNullOrEmpty(nombre) ? "El nombre completo es obligatorio." : null;
            ErrorCorreo = string.IsNullOrEmpty(correo)
                ? "El correo electrónico es obligatorio."
                : !CorreoValido(correo) ? "El formato del correo electrónico no es válido." : null;
            ErrorNombreUsuario = string.IsNullOrEmpty(nombreUsuario) ? "El nombre de usuario es obligatorio." : null;
            ErrorContrasenia = contrasenia.Length < LargoMinimoContrasenia
                ? $"La contraseña debe tener al menos {LargoMinimoContrasenia} caracteres."
                : null;
            ErrorConfirmacionContrasenia = contrasenia != (ConfirmacionContrasenia ?? string.Empty)
                ? "Las contraseñas no coinciden."
                : null;
            ErrorRol = rol is null ? "Debe seleccionar un rol." : null;

            if (HayErrorNombre || HayErrorCorreo || HayErrorNombreUsuario || HayErrorContrasenia
                || HayErrorConfirmacionContrasenia || HayErrorRol)
                return;

            // R6: crear un Administrador pide doble confirmación.
            // E3: si rechaza cualquiera de las dos, no se envía y queda en el formulario
            var confirmar = false;
            if (rol == RolUsuario.Administrador)
            {
                if (!await Shell.Current.DisplayAlertAsync("Crear administrador",
                        $"¿Desea crear a {nombre} como Administrador?", "Sí", "No"))
                    return;

                if (!await Shell.Current.DisplayAlertAsync("Confirmar administrador",
                        "El Administrador tendrá acceso total al sistema. ¿Confirma la creación?", "Confirmar", "Cancelar"))
                    return;

                confirmar = true;
            }

            var pedido = new CrearUsuarioRequest
            {
                Nombre = nombre,
                Correo = correo,
                NombreUsuario = nombreUsuario,
                Contrasenia = contrasenia,
                Rol = rol!.Value,
                Confirmar = confirmar
            };

            var creado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _usuarioService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    creado = true;
                    return;
                }

                // E2: el back avisa correo o nombre de usuario en uso con un 400 y su mensaje.
                // E4: cualquier otro error. En los dos casos el formulario conserva lo cargado
                MensajeError = resultado.CodigoEstado == HttpStatusCode.BadRequest
                    ? resultado.MensajeError
                    : MensajeErrorCreacion;
            });

            if (!creado)
                return;

            await Shell.Current.DisplayAlertAsync("Nuevo usuario", "Usuario creado correctamente.", "Aceptar");

            // S2: la lista se recarga al volver (UsuariosPage.OnAppearing) y muestra la fila nueva
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        // R2: mismo criterio que la API (ValidarFormatoCorreo en UsuarioController)
        private static bool CorreoValido(string correo) =>
            MailAddress.TryCreate(correo, out var direccion) && direccion.Address == correo;
    }
}
