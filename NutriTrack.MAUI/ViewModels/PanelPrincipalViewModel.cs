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
        private readonly IAlertaService _alertaService;

        [ObservableProperty]
        public partial string Nombre { get; set; } = string.Empty;

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

        // Tarjeta de alertas: solo Administrador. Para los otros roles no se muestra ni se llama a la API
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(IrAAlertasCommand))]
        public partial bool PuedeVerAlertas { get; set; }

        // Estado propio de la tarjeta de alertas, separado de EstaOcupado y MensajeError
        // (que usa la tarjeta de animales): un error de una no pisa el de la otra

        // Cuántas alertas se muestran en el pantallazo de la tarjeta (las más próximas, en el orden del back)
        public const int CantidadPantallazo = 3;

        // null mientras no se pudo cargar
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayAlertas))]
        [NotifyPropertyChangedFor(nameof(SinAlertas))]
        [NotifyPropertyChangedFor(nameof(HayMasAlertas))]
        [NotifyPropertyChangedFor(nameof(TextoVerTodas))]
        public partial int? TotalAlertas { get; set; }

        // Resalta la tarjeta y la campana, y muestra el pantallazo: con 0 se ve normal
        public bool HayAlertas => TotalAlertas > 0;

        // Cargó bien y no hay ninguna (null, mientras carga o con error, no cuenta)
        public bool SinAlertas => TotalAlertas == 0;

        // Pantallazo: primeras alertas de la misma respuesta que el contador (no hay otro pedido)
        [ObservableProperty]
        public partial IReadOnlyList<Alerta> PrimerasAlertas { get; set; } = [];

        public bool HayMasAlertas => TotalAlertas > CantidadPantallazo;

        public string TextoVerTodas => $"Ver las {TotalAlertas} alertas";

        // "No hay alertas para los próximos N días", con el rango del back
        [ObservableProperty]
        public partial string? MensajeSinAlertas { get; set; }

        [ObservableProperty]
        public partial bool CargandoAlertas { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorAlertas))]
        public partial string? ErrorAlertas { get; set; }

        public bool HayErrorAlertas => !string.IsNullOrEmpty(ErrorAlertas);

        public PanelPrincipalViewModel(ISesionService sesionService, IAnimalService animalService,
            IAlertaService alertaService)
        {
            _sesionService = sesionService;
            _animalService = animalService;
            _alertaService = alertaService;
        }

        // Se ejecuta cada vez que aparece el panel: Shell reutiliza la página,
        // así que después de cerrar sesión puede entrar otro usuario con otro rol
        [RelayCommand]
        private async Task Cargar()
        {
            var sesion = _sesionService.SesionActual;
            if (sesion is null)
                return;

            // Una sesión guardada antes de que la API devolviera el nombre no lo
            // tiene: hasta volver a iniciar sesión se muestra el nombre de usuario
            Nombre = string.IsNullOrWhiteSpace(sesion.Nombre) ? sesion.NombreUsuario : sesion.Nombre;
            Rol = TextoRol(sesion.Rol);

            // El menú no depende de la API: se arma antes de la tarjeta
            // para que quede usable aunque la tarjeta falle
            EntradasMenu = MenuModulos.ObtenerPara(sesion.Rol);
            PuedeVerAnimales = EntradasMenu.Any(e => e.Ruta == MenuModulos.Animales);
            PuedeVerAlertas = sesion.Rol == RolUsuario.Administrador;

            // En secuencia y no en paralelo: si el token venció, el primer 401 cierra la sesión
            // y el segundo pedido ya no vuelve a avisar (en paralelo saldrían dos carteles)
            await CargarTarjetaAnimalesAsync();
            await CargarTarjetaAlertasAsync();
        }

        // No usa EjecutarAsync: ese limpia MensajeError y usa EstaOcupado, que son de la tarjeta de animales
        private async Task CargarTarjetaAlertasAsync()
        {
            if (CargandoAlertas)
                return;

            TotalAlertas = null;
            ErrorAlertas = null;
            PrimerasAlertas = [];
            MensajeSinAlertas = null;

            // Otro rol, o la sesión se cerró por un 401 en la tarjeta de animales: no se llama a la API
            if (!PuedeVerAlertas || _sesionService.SesionActual is null)
                return;

            try
            {
                CargandoAlertas = true;

                var resultado = await _alertaService.ListarAsync();
                if (!resultado.Exito)
                {
                    ErrorAlertas = resultado.MensajeError;
                    return;
                }

                PrimerasAlertas = (resultado.Datos?.Alertas ?? []).Take(CantidadPantallazo).ToList();
                MensajeSinAlertas = resultado.Datos?.TextoSinAlertas;
                TotalAlertas = resultado.Datos?.Total ?? 0;
            }
            finally
            {
                CargandoAlertas = false;
            }
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

        // La lista de alertas no está en el menú: se entra solo desde la tarjeta
        [RelayCommand(CanExecute = nameof(PuedeVerAlertas))]
        private Task IrAAlertas() =>
            Shell.Current.GoToAsync(AlertasViewModel.Ruta);

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
