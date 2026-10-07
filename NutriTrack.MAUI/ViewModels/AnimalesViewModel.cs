using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Issue #97 - Lista de animales (no tiene CU propio: manda el prototipo)
    public partial class AnimalesViewModel : BaseViewModel
    {
        // Prueba 3 del issue: bajar a 2 para probar "Cargar más" y volver a 50
        private const int TamanioPagina = 50;

        // Milisegundos sin tipear antes de buscar
        private const int EsperaBusquedaMs = 400;

        private readonly IAnimalService _animalService;
        private readonly ISesionService _sesionService;

        // Última página pedida a la API
        private int _pagina;

        // Espera de la búsqueda mientras se escribe (se cancela con cada letra)
        private CancellationTokenSource? _esperaBusqueda;

        public AnimalesViewModel(IAnimalService animalService, ISesionService sesionService)
        {
            _animalService = animalService;
            _sesionService = sesionService;
        }

        // Animales cargados hasta ahora (se van sumando con "Cargar más")
        public ObservableCollection<AnimalListado> Animales { get; } = [];

        // La búsqueda va contra el servidor (parámetro texto de la API)
        [ObservableProperty]
        public partial string? TextoBusqueda { get; set; }

        // Total de animales que cumplen el filtro, según la API
        [ObservableProperty]
        public partial int Total { get; set; }

        // "N de total"
        [ObservableProperty]
        public partial string Contador { get; set; } = string.Empty;

        // Quedan animales por traer
        [ObservableProperty]
        public partial bool HayMas { get; set; }

        // "Agregar animal": solo Encargado y Administrador
        [ObservableProperty]
        public partial bool PuedeAgregar { get; set; }

        // Mensaje cuando la lista queda vacía (un 200 sin animales no es un error)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        // Busca mientras se escribe: espera a que se deje de tipear
        // para no hacer un pedido a la API por cada letra.
        // Si el texto queda vacío, vuelve la lista completa
        partial void OnTextoBusquedaChanged(string? value) => _ = BuscarConEsperaAsync();

        private async Task BuscarConEsperaAsync()
        {
            // Cada letra nueva cancela la espera de la anterior
            _esperaBusqueda?.Cancel();
            var espera = _esperaBusqueda = new CancellationTokenSource();

            try
            {
                await Task.Delay(EsperaBusquedaMs, espera.Token);

                // Si todavía está cargando la búsqueda anterior, espera a que termine
                while (EstaOcupado)
                    await Task.Delay(50, espera.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await CargarCommand.ExecuteAsync(null);
        }

        // Primera página: al entrar a la pantalla y al buscar
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            // Si se buscó con Enter o el botón, no hace falta la búsqueda pendiente
            _esperaBusqueda?.Cancel();

            var rol = _sesionService.SesionActual?.Rol;
            PuedeAgregar = rol is RolUsuario.EncargadoDeCampo or RolUsuario.Administrador;

            // Sin incluirInactivos: por defecto solo vienen los activos
            var resultado = await _animalService.ListarAsync(TextoBusqueda, tamanioPagina: TamanioPagina);

            Animales.Clear();
            _pagina = 1;

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
            var resultado = await _animalService.ListarAsync(
                TextoBusqueda, pagina: _pagina + 1, tamanioPagina: TamanioPagina);

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
        private async Task SeleccionarAsync(AnimalListado animal)
        {
            // TODO #104: cuando exista el perfil, navegar pasando id y caravana:
            // await Shell.Current.GoToAsync("perfilAnimal", new Dictionary<string, object>
            // {
            //     ["id"] = animal.Id,
            //     ["caravanaCuig"] = animal.CaravanaCuig,
            //     ["caravanaNroManejo"] = animal.CaravanaNroManejo
            // });
            await Shell.Current.DisplayAlertAsync("Perfil del animal", "Pantalla en construcción", "Aceptar");
        }

        [RelayCommand]
        private Task AgregarAnimalAsync() => Shell.Current.GoToAsync(AgregarAnimalViewModel.Ruta);

        private void ActualizarEstado()
        {
            Contador = $"{Animales.Count} de {Total}";
            HayMas = Animales.Count < Total;
            MensajeVacio = HayError ? null : "No se encontraron animales";
        }
    }
}
