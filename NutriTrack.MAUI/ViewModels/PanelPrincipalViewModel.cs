using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    public partial class PanelPrincipalViewModel : BaseViewModel
    {
        private readonly ISesionService _sesionService;
        private readonly IAnimalService _animalService;

        [ObservableProperty]
        public partial string NombreUsuario { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Rol { get; set; } = string.Empty;

        // Opciones del menú que ve el rol de la sesión
        [ObservableProperty]
        public partial IReadOnlyList<EntradaMenu> EntradasMenu { get; set; } = [];

        // La tarjeta de animales solo navega si el rol tiene Animales en su menú
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(IrAAnimalesCommand))]
        public partial bool PuedeVerAnimales { get; set; }

        // Tarjeta de animales: null mientras no se pudo cargar
        [ObservableProperty]
        public partial int? AnimalesActivos { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarRegistrados))]
        public partial int? AnimalesRegistrados { get; set; }

        // Si el rol no puede consultar los inactivos, se muestran solo los activos
        public bool MostrarRegistrados => AnimalesRegistrados.HasValue;

        public PanelPrincipalViewModel(ISesionService sesionService, IAnimalService animalService)
        {
            _sesionService = sesionService;
            _animalService = animalService;
        }

        // Se ejecuta cada vez que aparece el panel: Shell reutiliza la página,
        // así que después de cerrar sesión puede entrar otro usuario con otro rol
        [RelayCommand]
        private async Task Cargar()
        {
            var sesion = _sesionService.SesionActual;
            if (sesion is null)
                return;

            NombreUsuario = sesion.NombreUsuario;
            Rol = TextoRol(sesion.Rol);

            // El menú no depende de la API: se arma antes de la tarjeta
            // para que quede usable aunque la tarjeta falle
            EntradasMenu = MenuModulos.ObtenerPara(sesion.Rol);
            PuedeVerAnimales = EntradasMenu.Any(e => e.Ruta == MenuModulos.Animales);

            await CargarTarjetaAnimalesAsync();
        }

        private Task CargarTarjetaAnimalesAsync() => EjecutarAsync(async () =>
        {
            AnimalesActivos = null;
            AnimalesRegistrados = null;

            // Solo interesa el total, por eso se pide una página de 1 elemento
            var activos = await _animalService.ListarAsync(tamanioPagina: 1);
            if (!activos.Exito)
            {
                MensajeError = activos.MensajeError;
                return;
            }

            AnimalesActivos = activos.Datos?.Total ?? 0;

            var registrados = await _animalService.ListarAsync(tamanioPagina: 1, incluirInactivos: true);
            if (registrados.Exito)
            {
                AnimalesRegistrados = registrados.Datos?.Total ?? 0;
                return;
            }

            // 403: el rol no puede ver los inactivos; la tarjeta queda solo con los activos
            if (registrados.CodigoEstado != HttpStatusCode.Forbidden)
                MensajeError = registrados.MensajeError;
        });

        [RelayCommand]
        private Task Navegar(EntradaMenu entrada) =>
            Shell.Current.GoToAsync(entrada.Ruta);

        [RelayCommand(CanExecute = nameof(PuedeVerAnimales))]
        private Task IrAAnimales() =>
            Shell.Current.GoToAsync(MenuModulos.Animales);

        [RelayCommand]
        private async Task CerrarSesion()
        {
            _sesionService.CerrarSesion();

            // "//" reemplaza el historial: con "atrás" no se vuelve al panel
            await Shell.Current.GoToAsync("//login");
        }

        private static string TextoRol(RolUsuario rol) => rol switch
        {
            RolUsuario.Administrador => "Administrador",
            RolUsuario.EncargadoDeCampo => "Encargado de campo",
            RolUsuario.AsesorTecnico => "Asesor técnico",
            _ => rol.ToString()
        };
    }
}
