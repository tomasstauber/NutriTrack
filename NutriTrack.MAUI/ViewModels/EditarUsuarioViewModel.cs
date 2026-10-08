using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU22 - Editar usuario
    // Se llega desde "Editar" de la lista de usuarios (solo Administrador).
    // Recibe por navegación "usuario" (la fila de la lista): no hay GET de usuario por id
    public partial class EditarUsuarioViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "editar-usuario";

        private const string MensajeErrorEdicion = "No se pudo editar el usuario; intente nuevamente.";

        private readonly IUsuarioService _usuarioService;
        private readonly ISesionService _sesionService;

        // Datos de la fila recibida; el rol original se usa para R6
        private int _id;
        private RolUsuario _rolOriginal;

        public EditarUsuarioViewModel(IUsuarioService usuarioService, ISesionService sesionService)
        {
            _usuarioService = usuarioService;
            _sesionService = sesionService;
        }

        // Los tres roles con su texto legible
        public IReadOnlyList<OpcionRol> OpcionesRol { get; } =
        [
            new() { Texto = "Administrador", Rol = RolUsuario.Administrador },
            new() { Texto = "Encargado de campo", Rol = RolUsuario.EncargadoDeCampo },
            new() { Texto = "Asesor técnico", Rol = RolUsuario.AsesorTecnico }
        ];

        // Datos del usuario (no hay contraseña: el DTO de edición no la tiene)

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Correo { get; set; }

        [ObservableProperty]
        public partial string? NombreUsuario { get; set; }

        [ObservableProperty]
        public partial OpcionRol? RolSeleccionado { get; set; }

        // R7: editando al propio usuario no se puede cambiar el rol
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PuedeCambiarRol))]
        public partial bool EsUsuarioActual { get; set; }

        public bool PuedeCambiarRol => !EsUsuarioActual;

        // Error de cada campo (null si el campo está bien).
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
        [NotifyPropertyChangedFor(nameof(HayErrorRol))]
        public partial string? ErrorRol { get; set; }

        public bool HayErrorNombre => !string.IsNullOrEmpty(ErrorNombre);
        public bool HayErrorCorreo => !string.IsNullOrEmpty(ErrorCorreo);
        public bool HayErrorNombreUsuario => !string.IsNullOrEmpty(ErrorNombreUsuario);
        public bool HayErrorRol => !string.IsNullOrEmpty(ErrorRol);

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("usuario", out var valor) || valor is not Usuario usuario)
                return;

            // Precarga con los datos de la fila
            _id = usuario.Id;
            _rolOriginal = usuario.Rol;
            Nombre = usuario.Nombre;
            Correo = usuario.Correo;
            NombreUsuario = usuario.NombreUsuario;
            RolSeleccionado = OpcionesRol.FirstOrDefault(o => o.Rol == usuario.Rol);

            // R7: se compara el nombre de usuario de la fila con el de la sesión
            EsUsuarioActual = string.Equals(usuario.NombreUsuario, _sesionService.SesionActual?.NombreUsuario,
                StringComparison.OrdinalIgnoreCase);
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            var nombre = Nombre?.Trim() ?? string.Empty;
            var correo = Correo?.Trim() ?? string.Empty;
            var nombreUsuario = NombreUsuario?.Trim() ?? string.Empty;
            var rol = RolSeleccionado?.Rol;

            // R2 a R5: las mismas validaciones del alta, todas a la vez
            ErrorNombre = UsuarioValidador.ValidarNombre(nombre);
            ErrorCorreo = UsuarioValidador.ValidarCorreo(correo);
            ErrorNombreUsuario = UsuarioValidador.ValidarNombreUsuario(nombreUsuario);
            ErrorRol = UsuarioValidador.ValidarRol(rol);

            if (HayErrorNombre || HayErrorCorreo || HayErrorNombreUsuario || HayErrorRol)
                return;

            // R6: pasar a Administrador pide doble confirmación.
            // E2: si rechaza cualquiera de las dos, no se guarda y queda en el formulario
            var confirmar = false;
            if (rol == RolUsuario.Administrador && _rolOriginal != RolUsuario.Administrador)
            {
                if (!await Shell.Current.DisplayAlertAsync("Cambiar a administrador",
                        $"¿Desea asignar a {nombre} el rol de Administrador?", "Sí", "No"))
                    return;

                if (!await Shell.Current.DisplayAlertAsync("Confirmar administrador",
                        "El Administrador tendrá acceso total al sistema. ¿Confirma el cambio?", "Confirmar", "Cancelar"))
                    return;

                confirmar = true;
            }

            var pedido = new EditarUsuarioRequest
            {
                Nombre = nombre,
                Correo = correo,
                NombreUsuario = nombreUsuario,
                Rol = rol!.Value,
                Confirmar = confirmar
            };

            var editado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _usuarioService.EditarAsync(_id, pedido);

                if (resultado.Exito)
                {
                    editado = true;
                    return;
                }

                // E1 (correo o nombre de usuario repetidos) y E3 (propio rol): el back responde 400 con su mensaje.
                // E4: cualquier otro error. En los dos casos el formulario conserva lo cargado
                MensajeError = resultado.CodigoEstado == HttpStatusCode.BadRequest
                    ? resultado.MensajeError
                    : MensajeErrorEdicion;
            });

            if (!editado)
                return;

            await Shell.Current.DisplayAlertAsync("Editar usuario", "Usuario editado correctamente.", "Aceptar");

            // La lista se recarga al volver (UsuariosPage.OnAppearing) y muestra la fila actualizada
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
