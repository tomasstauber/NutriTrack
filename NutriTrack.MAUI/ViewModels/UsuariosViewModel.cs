using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU20 - Consultar usuarios y CU22 - Editar usuario
    public partial class UsuariosViewModel : BaseViewModel
    {
        private const string MensajeSinResultados = "No se encontraron usuarios con los filtros seleccionados.";
        private const string MensajeErrorCarga = "No se pudieron cargar los usuarios; intente nuevamente.";

        private readonly IUsuarioService _usuarioService;

        // Lista completa tal como vino de la API; los filtros se aplican sobre esta lista
        private List<Usuario> _todos = [];

        public UsuariosViewModel(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
            RolSeleccionado = OpcionesRol[0];
        }

        // Lo que se muestra en la tabla (lista filtrada)
        public ObservableCollection<Usuario> Usuarios { get; } = [];

        // R3: "Todos" y los tres roles
        public IReadOnlyList<OpcionRol> OpcionesRol { get; } =
        [
            new() { Texto = "Todos", Rol = null },
            new() { Texto = "Administrador", Rol = RolUsuario.Administrador },
            new() { Texto = "Encargado de campo", Rol = RolUsuario.EncargadoDeCampo },
            new() { Texto = "Asesor técnico", Rol = RolUsuario.AsesorTecnico }
        ];

        [ObservableProperty]
        public partial string? TextoBusqueda { get; set; }

        [ObservableProperty]
        public partial OpcionRol? RolSeleccionado { get; set; }

        // S2: total de usuarios que se ven en la tabla
        [ObservableProperty]
        public partial string TextoTotal { get; set; } = string.Empty;

        // Mensaje cuando la tabla queda vacía (E1)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        // E2: el botón Reintentar solo tiene sentido si falló la carga (no con un 403)
        [ObservableProperty]
        public partial bool PuedeReintentar { get; set; }

        partial void OnTextoBusquedaChanged(string? value) => Filtrar();

        partial void OnRolSeleccionadoChanged(OpcionRol? value) => Filtrar();

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            PuedeReintentar = false;

            var resultado = await _usuarioService.ListarAsync();

            if (resultado.Exito)
            {
                _todos = resultado.Datos ?? [];
            }
            else
            {
                _todos = [];

                // R1: un 403 muestra el mensaje de permisos; cualquier otro error es E2
                if (resultado.CodigoEstado == HttpStatusCode.Forbidden)
                {
                    MensajeError = resultado.MensajeError;
                }
                else
                {
                    MensajeError = MensajeErrorCarga;
                    PuedeReintentar = true;
                }
            }

            Filtrar();
        });

        // CU21: la lista se recarga al volver del formulario (UsuariosPage.OnAppearing)
        [RelayCommand]
        private Task NuevoUsuario() =>
            Shell.Current.GoToAsync(NuevoUsuarioViewModel.Ruta);

        // CU22: el formulario se precarga con la fila (no hay GET de usuario por id).
        // La lista se recarga al volver (UsuariosPage.OnAppearing)
        [RelayCommand]
        private Task Editar(Usuario usuario) =>
            Shell.Current.GoToAsync(EditarUsuarioViewModel.Ruta, new Dictionary<string, object>
            {
                ["usuario"] = usuario
            });

        // R2 y R3: filtros en memoria sobre la lista ya cargada (el endpoint no tiene parámetros)
        private void Filtrar()
        {
            var texto = TextoBusqueda?.Trim();
            var rol = RolSeleccionado?.Rol;

            var filtrados = _todos
                .Where(u => string.IsNullOrEmpty(texto)
                    || u.Nombre.Contains(texto, StringComparison.CurrentCultureIgnoreCase)
                    || u.NombreUsuario.Contains(texto, StringComparison.CurrentCultureIgnoreCase)
                    || u.Correo.Contains(texto, StringComparison.CurrentCultureIgnoreCase))
                .Where(u => rol is null || u.Rol == rol)
                .ToList();

            Usuarios.Clear();
            foreach (var usuario in filtrados)
                Usuarios.Add(usuario);

            if (HayError)
            {
                TextoTotal = string.Empty;
                MensajeVacio = null;
            }
            else
            {
                TextoTotal = $"Total de usuarios: {Usuarios.Count}";
                MensajeVacio = MensajeSinResultados;
            }
        }
    }
}
