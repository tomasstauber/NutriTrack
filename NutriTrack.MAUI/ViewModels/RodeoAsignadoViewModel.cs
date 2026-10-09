using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Issue #159 - Rodeo asignado a un plan: datos del rodeo, vigencia y sus animales activos.
    // Recibe por navegación "asignacion" desde la ficha del plan (no hay GET de un rodeo por id)
    public partial class RodeoAsignadoViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell de la pantalla (se registra en AppShell.xaml.cs)
        public const string Ruta = "rodeo-asignado";

        private const int TamanioPagina = 50;

        private readonly IAnimalService _animalService;

        // Última página pedida a la API
        private int _pagina;

        public RodeoAsignadoViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
        }

        [ObservableProperty]
        public partial AsignacionActivaPlan? Asignacion { get; set; }

        // Animales cargados hasta ahora (se van sumando con "Cargar más")
        public ObservableCollection<AnimalListado> Animales { get; } = [];

        // Total de animales activos del rodeo, según la API
        [ObservableProperty]
        public partial int Total { get; set; }

        // "N de total"
        [ObservableProperty]
        public partial string Contador { get; set; } = string.Empty;

        // Quedan animales por traer
        [ObservableProperty]
        public partial bool HayMas { get; set; }

        // Mensaje cuando la lista queda vacía (un 200 sin animales no es un error)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("asignacion", out var asignacion))
                Asignacion = asignacion as AsignacionActivaPlan;
        }

        // Primera página: al entrar a la pantalla
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            Animales.Clear();
            _pagina = 1;

            if (Asignacion is null)
            {
                Total = 0;
                MensajeError = "No se indicó el rodeo.";
                ActualizarEstado();
                return;
            }

            // Sin incluirInactivos: solo vienen los activos
            var resultado = await _animalService.ListarAsync(
                texto: null, idRodeo: Asignacion.IdRodeo, tamanioPagina: TamanioPagina);

            if (resultado.Exito)
            {
                Total = resultado.Datos?.Total ?? 0;
                foreach (var animal in resultado.Datos?.Items ?? [])
                    Animales.Add(animal);
            }
            else
            {
                Total = 0;
                MensajeError = resultado.MensajeError;
            }

            ActualizarEstado();
        });

        // Página siguiente, sumada a lo que ya está cargado
        [RelayCommand]
        private Task CargarMasAsync() => EjecutarAsync(async () =>
        {
            if (Asignacion is null)
                return;

            var resultado = await _animalService.ListarAsync(
                texto: null, idRodeo: Asignacion.IdRodeo, pagina: _pagina + 1, tamanioPagina: TamanioPagina);

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                return;
            }

            _pagina++;
            Total = resultado.Datos?.Total ?? Total;
            foreach (var animal in resultado.Datos?.Items ?? [])
                Animales.Add(animal);

            ActualizarEstado();
        });

        [RelayCommand]
        private Task SeleccionarAsync(AnimalListado animal) =>
            // El perfil pide la ficha por caravana y el historial por id
            Shell.Current.GoToAsync(PerfilAnimalViewModel.Ruta, new Dictionary<string, object>
            {
                ["id"] = animal.Id,
                ["caravanaCuig"] = animal.CaravanaCuig,
                ["caravanaNroManejo"] = animal.CaravanaNroManejo
            });

        private void ActualizarEstado()
        {
            Contador = $"{Animales.Count} de {Total}";
            HayMas = Animales.Count < Total;
            MensajeVacio = HayError ? null : "El rodeo no tiene animales activos";
        }
    }
}
