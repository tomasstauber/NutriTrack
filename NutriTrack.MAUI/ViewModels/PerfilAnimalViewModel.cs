using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU2 - Consulta de ficha individual y CU4 - Desactivar ficha de un animal.
    // El historial sanitario viene del prototipo.
    // Recibe por navegación "id", "caravanaCuig" y "caravanaNroManejo":
    // la ficha se pide por caravana y el historial por id
    public partial class PerfilAnimalViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del perfil (se registra en AppShell.xaml.cs)
        public const string Ruta = "perfilAnimal";

        private readonly IAnimalService _animalService;
        private readonly IEventoSanitarioService _eventoSanitarioService;
        private readonly ISesionService _sesionService;

        // Datos recibidos por navegación
        private int? _idAnimal;
        private string? _cuig;
        private string? _nroManejo;

        public PerfilAnimalViewModel(IAnimalService animalService,
            IEventoSanitarioService eventoSanitarioService, ISesionService sesionService)
        {
            _animalService = animalService;
            _eventoSanitarioService = eventoSanitarioService;
            _sesionService = sesionService;
        }

        // Ficha del animal (null hasta cargarla o si falló)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayFicha))]
        [NotifyPropertyChangedFor(nameof(Caravana))]
        [NotifyPropertyChangedFor(nameof(TextoPesoAlNacer))]
        [NotifyPropertyChangedFor(nameof(TextoUltimoPeso))]
        [NotifyPropertyChangedFor(nameof(ObservacionesUltimoPeso))]
        [NotifyPropertyChangedFor(nameof(EstaActivo))]
        [NotifyPropertyChangedFor(nameof(PuedeRegistrarPeso))]
        [NotifyPropertyChangedFor(nameof(PuedeDesactivar))]
        public partial FichaAnimal? Ficha { get; set; }

        public bool HayFicha => Ficha is not null;

        public string Caravana => Ficha is null
            ? string.Empty
            : CaravanaValidador.Formatear(Ficha.CaravanaCuig, Ficha.CaravanaNroManejo);

        public string TextoPesoAlNacer => Ficha is null
            ? string.Empty
            : $"{FormatearPeso(Ficha.PesoAlNacer)} kg";

        // La fecha llega con Z y se muestra tal cual, sin pasar a hora local
        public string TextoUltimoPeso => Ficha?.UltimoPeso is { } ultimo
            ? $"{FormatearPeso(ultimo.PesoKg)} kg ({ultimo.FechaPesaje:dd/MM/yyyy})"
            : "Sin pesajes";

        public string? ObservacionesUltimoPeso => Ficha?.UltimoPeso?.Observaciones;

        public bool EstaActivo => Ficha?.Estado == "Activo";

        // Editar, Registrar peso y Desactivar: solo Encargado y Administrador
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PuedeRegistrarPeso))]
        [NotifyPropertyChangedFor(nameof(PuedeDesactivar))]
        public partial bool PuedeGestionar { get; set; }

        // Con el animal inactivo no se muestran Registrar peso ni Desactivar
        public bool PuedeRegistrarPeso => PuedeGestionar && EstaActivo;

        public bool PuedeDesactivar => PuedeGestionar && EstaActivo;

        // Historial sanitario, del más reciente al más viejo (así llega)
        public ObservableCollection<EventoHistorial> Eventos { get; } = [];

        // Error del historial, aparte del de la ficha: si falla el historial, la ficha igual se ve
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorHistorial))]
        public partial string? MensajeErrorHistorial { get; set; }

        public bool HayErrorHistorial => !string.IsNullOrEmpty(MensajeErrorHistorial);

        // "Sin eventos sanitarios registrados": solo con un historial vacío que llegó bien
        [ObservableProperty]
        public partial bool HistorialVacio { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("id", out var id) && id is int idAnimal)
                _idAnimal = idAnimal;

            if (query.TryGetValue("caravanaCuig", out var cuig))
                _cuig = cuig?.ToString();

            if (query.TryGetValue("caravanaNroManejo", out var nroManejo))
                _nroManejo = nroManejo?.ToString();
        }

        // Al aparecer: ficha e historial, cada uno con su propio error
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            // Shell puede reutilizar la página: el permiso se calcula en cada carga
            var rol = _sesionService.SesionActual?.Rol;
            PuedeGestionar = rol is RolUsuario.EncargadoDeCampo or RolUsuario.Administrador;

            await CargarFichaAsync();
            await CargarHistorialAsync();
        });

        private async Task CargarFichaAsync()
        {
            if (string.IsNullOrEmpty(_cuig) || string.IsNullOrEmpty(_nroManejo))
            {
                Ficha = null;
                MensajeError = "No se indicó la caravana del animal.";
                return;
            }

            var resultado = await _animalService.ObtenerFichaAsync(_cuig, _nroManejo);

            if (!resultado.Exito)
            {
                Ficha = null;
                MensajeError = resultado.MensajeError;
                return;
            }

            Ficha = resultado.Datos;
        }

        private async Task CargarHistorialAsync()
        {
            Eventos.Clear();
            MensajeErrorHistorial = null;
            HistorialVacio = false;

            if (_idAnimal is null)
            {
                MensajeErrorHistorial = "No se indicó el id del animal.";
                return;
            }

            var resultado = await _eventoSanitarioService.ObtenerHistorialAnimalAsync(_idAnimal.Value);

            if (!resultado.Exito)
            {
                MensajeErrorHistorial = resultado.MensajeError;
                return;
            }

            // Un [] no es un error: se muestra el mensaje de historial vacío
            foreach (var evento in resultado.Datos ?? [])
                Eventos.Add(evento);

            HistorialVacio = Eventos.Count == 0;
        }

        // Formulario de edición con la caravana del animal: precarga la ficha solo.
        // Al volver, el perfil se recarga (OnAppearing) y muestra los datos nuevos
        [RelayCommand]
        private Task EditarAsync()
        {
            if (Ficha is null)
                return Task.CompletedTask;

            return Shell.Current.GoToAsync(EditarAnimalViewModel.Ruta, new Dictionary<string, object>
            {
                ["caravanaCuig"] = Ficha.CaravanaCuig,
                ["caravanaNroManejo"] = Ficha.CaravanaNroManejo
            });
        }

        // Pantalla de peso con la caravana precargada: busca el animal sola
        [RelayCommand]
        private Task RegistrarPesoAsync()
        {
            if (Ficha is null)
                return Task.CompletedTask;

            return Shell.Current.GoToAsync(MenuModulos.Peso, new Dictionary<string, object>
            {
                ["caravanaCuig"] = Ficha.CaravanaCuig,
                ["caravanaNroManejo"] = Ficha.CaravanaNroManejo
            });
        }

        [RelayCommand]
        private async Task DesactivarAsync()
        {
            if (Ficha is null)
                return;

            var cuig = Ficha.CaravanaCuig;
            var nroManejo = Ficha.CaravanaNroManejo;

            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Desactivar animal",
                $"¿Desactivar el animal {Caravana}? Deja de aparecer en la lista de animales activos.",
                "Desactivar",
                "Cancelar");

            // CU4 E4: si cancela, no se llama a la API
            if (!confirmado)
                return;

            var desactivado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _animalService.DesactivarAsync(cuig, nroManejo);

                if (resultado.Exito)
                    desactivado = true;
                else
                    // CU4 E3: "El animal ya está inactivo." (400), u otro mensaje del back
                    MensajeError = resultado.MensajeError;
            });

            if (!desactivado)
                return;

            await Shell.Current.DisplayAlertAsync("Desactivar animal",
                "El animal fue desactivado correctamente.", "Aceptar");

            // La ficha ahora dice Inactivo y se ocultan Registrar peso y Desactivar
            await EjecutarAsync(CargarFichaAsync);
        }

        private static string FormatearPeso(decimal pesoKg) => pesoKg.ToString("0.##");
    }
}
