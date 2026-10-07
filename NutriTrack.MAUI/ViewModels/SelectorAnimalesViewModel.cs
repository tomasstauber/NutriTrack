using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Issue #101 - Selector múltiple de animales (componente, no una página).
    // Lo usan Crear rodeo (CU6), Transferir animales (CU7) y Evento sanitario (CU12).
    //
    // Uso desde la pantalla anfitriona:
    //   1. Recibir un SelectorAnimalesViewModel por constructor y exponerlo como propiedad.
    //   2. Configurar el origen con CambiarOrigenAsync(OrigenAnimales.SinRodeo / DeRodeo(id) / Todos).
    //   3. Leer Seleccionados (animales completos) o CantidadSeleccionados.
    //   4. Después de guardar algo que cambie los animales, llamar a RecargarAsync().
    // El selector no valida mínimos ("al menos uno"): eso lo hace la anfitriona.
    public partial class SelectorAnimalesViewModel : BaseViewModel
    {
        // El máximo que acepta el back. Prueba 3 del issue: bajar a 2 y volver a 100
        private const int TamanioPagina = 100;

        // Milisegundos sin tipear antes de buscar
        private const int EsperaBusquedaMs = 400;

        private const string MensajeSinAnimales = "No hay animales para mostrar";

        private readonly IAnimalService _animalService;

        // Selección guardada por id, con el animal completo: así sobrevive a buscar
        // y a cargar más, y se pueden devolver las caravanas de animales que ya no
        // están en la lista visible
        private readonly Dictionary<int, AnimalListado> _seleccionados = [];

        // Los mismos animales, en el orden en que se marcaron (lo que ve la anfitriona)
        private readonly ObservableCollection<AnimalListado> _listaSeleccionados = [];

        // Última página cargada de la lista visible
        private int _pagina;

        // Número de la carga actual de la lista: si llega la respuesta de una carga
        // vieja (por ejemplo, de antes de cambiar el origen), se ignora
        private int _cargaActual;

        // Igual que _cargaActual, para la verificación de la selección al recargar
        private int _verificacionActual;

        // Espera de la búsqueda mientras se escribe (se cancela con cada letra)
        private CancellationTokenSource? _esperaBusqueda;

        // Lo que repite el botón Reintentar después de un error
        private Func<Task>? _reintentar;

        public SelectorAnimalesViewModel(IAnimalService animalService)
        {
            _animalService = animalService;
            Seleccionados = new ReadOnlyObservableCollection<AnimalListado>(_listaSeleccionados);
        }

        // ===== Interfaz pública para la pantalla anfitriona =====

        // Origen actual; null hasta que la anfitriona llama a CambiarOrigenAsync
        private OrigenAnimales? _origen;
        public OrigenAnimales? Origen
        {
            get => _origen;
            private set => SetProperty(ref _origen, value);
        }

        // Animales seleccionados completos (Id, CaravanaCuig, CaravanaNroManejo...).
        // Crear rodeo y Transferir usan los ids; Evento sanitario, las caravanas
        public ReadOnlyObservableCollection<AnimalListado> Seleccionados { get; }

        public int CantidadSeleccionados => _listaSeleccionados.Count;

        // Configura de dónde salen los animales.
        // Si es otro origen, limpia la selección y recarga.
        // Si es el mismo, hace lo mismo que RecargarAsync (conserva la selección)
        public async Task CambiarOrigenAsync(OrigenAnimales origen)
        {
            ArgumentNullException.ThrowIfNull(origen);

            if (origen == Origen)
            {
                await RecargarAsync();
                return;
            }

            Origen = origen;
            LimpiarSeleccion();
            await CargarPrimeraPaginaAsync();
        }

        // Vuelve a pedir el mismo origen (por ejemplo, después de guardar).
        // Conserva marcados los animales que siguen en el origen y descarta los que ya no están
        public async Task RecargarAsync()
        {
            if (Origen is null)
                return;

            await CargarPrimeraPaginaAsync();
            await VerificarSeleccionAsync();
        }

        // ===== Para la parte visual del componente =====

        // Filas cargadas hasta ahora (se van sumando con "Cargar más")
        public ObservableCollection<AnimalSeleccionable> Animales { get; } = [];

        // La búsqueda va contra el servidor (parámetro texto de la API)
        [ObservableProperty]
        public partial string? TextoBusqueda { get; set; }

        // Total de animales del origen que cumplen la búsqueda, según la API
        [ObservableProperty]
        public partial int Total { get; set; }

        // Quedan animales por traer
        [ObservableProperty]
        public partial bool HayMas { get; set; }

        // Mensaje cuando la lista queda vacía (un 200 sin animales no es un error)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        // "N seleccionados"
        public string TextoSeleccionados => $"{CantidadSeleccionados} seleccionados";

        // Buscar NO descarta nada de la selección: solo cambia la lista visible
        partial void OnTextoBusquedaChanged(string? value) => _ = BuscarConEsperaAsync();

        [RelayCommand]
        private void AlternarSeleccion(AnimalSeleccionable fila)
        {
            var animal = fila.Animal;

            if (_seleccionados.Remove(animal.Id))
            {
                QuitarDeLista(animal.Id);
                fila.Seleccionado = false;
            }
            else
            {
                _seleccionados[animal.Id] = animal;
                _listaSeleccionados.Add(animal);
                fila.Seleccionado = true;
            }

            NotificarSeleccion();
        }

        [RelayCommand]
        private void LimpiarSeleccion()
        {
            _seleccionados.Clear();
            _listaSeleccionados.Clear();

            foreach (var fila in Animales)
                fila.Seleccionado = false;

            NotificarSeleccion();
        }

        // Página siguiente, sumada a lo que ya está cargado. NO descarta nada de la selección
        [RelayCommand]
        private async Task CargarMasAsync()
        {
            if (Origen is null || EstaOcupado || !HayMas)
                return;

            var carga = _cargaActual;
            EstaOcupado = true;
            MensajeError = null;

            try
            {
                var resultado = await ListarAsync(Origen, TextoBusqueda, _pagina + 1);
                if (carga != _cargaActual)
                    return;

                if (!resultado.Exito)
                {
                    MostrarError(resultado.MensajeError, CargarMasAsync);
                    return;
                }

                _pagina++;
                Total = resultado.Datos?.Total ?? Total;
                AgregarFilas(resultado.Datos?.Items ?? []);
                ActualizarEstado();
            }
            finally
            {
                if (carga == _cargaActual)
                    EstaOcupado = false;
            }
        }

        [RelayCommand]
        private async Task ReintentarAsync()
        {
            if (_reintentar is not null)
                await _reintentar();
        }

        // ===== Carga de la lista visible =====

        private async Task BuscarConEsperaAsync()
        {
            // Cada letra nueva cancela la espera de la anterior
            _esperaBusqueda?.Cancel();
            var espera = _esperaBusqueda = new CancellationTokenSource();

            try
            {
                await Task.Delay(EsperaBusquedaMs, espera.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            await CargarPrimeraPaginaAsync();
        }

        // Página 1 del origen con el texto actual. Reemplaza la lista visible,
        // pero no toca la selección: las filas se marcan según _seleccionados
        private async Task CargarPrimeraPaginaAsync()
        {
            if (Origen is null)
                return;

            // Si se recarga por otro motivo, la búsqueda pendiente ya no hace falta
            _esperaBusqueda?.Cancel();

            var carga = ++_cargaActual;
            EstaOcupado = true;
            MensajeError = null;

            try
            {
                var resultado = await ListarAsync(Origen, TextoBusqueda, 1);
                if (carga != _cargaActual)
                    return;

                Animales.Clear();
                _pagina = 1;

                if (!resultado.Exito)
                {
                    Total = 0;
                    MostrarError(resultado.MensajeError, CargarPrimeraPaginaAsync);
                }
                else
                {
                    Total = resultado.Datos?.Total ?? 0;
                    AgregarFilas(resultado.Datos?.Items ?? []);
                }

                ActualizarEstado();
            }
            finally
            {
                if (carga == _cargaActual)
                    EstaOcupado = false;
            }
        }

        // ===== Verificación de la selección al recargar =====

        // Con paginado, un animal marcado puede no estar en la página cargada y seguir
        // existiendo. Por eso no se mira la lista visible: se recorre el origen completo
        // (sin texto, de a TamanioPagina) hasta encontrar todos los marcados o terminar.
        // Si un pedido falla, no se descarta nada.
        private async Task VerificarSeleccionAsync()
        {
            var origen = Origen;
            if (origen is null || _seleccionados.Count == 0)
                return;

            var verificacion = ++_verificacionActual;
            var pendientes = new HashSet<int>(_seleccionados.Keys);
            var encontrados = new Dictionary<int, AnimalListado>();
            var pagina = 1;

            while (pendientes.Count > 0)
            {
                var resultado = await ListarAsync(origen, null, pagina);

                // Cambió el origen u otra recarga empezó una verificación nueva
                if (verificacion != _verificacionActual || origen != Origen)
                    return;

                if (!resultado.Exito)
                {
                    MostrarError(resultado.MensajeError, RecargarAsync);
                    return;
                }

                var datos = resultado.Datos;
                if (datos is null)
                    break;

                foreach (var animal in datos.Items)
                {
                    if (pendientes.Remove(animal.Id))
                        encontrados[animal.Id] = animal;
                }

                if (datos.Items.Count == 0 || pagina * TamanioPagina >= datos.Total)
                    break;

                pagina++;
            }

            // Los que siguen: se actualizan con los datos nuevos (por ejemplo, el rodeo)
            foreach (var (id, animal) in encontrados)
                ReemplazarSeleccionado(id, animal);

            // Los que no aparecieron ya no están en el origen: se descartan.
            // Solo se miran los que estaban marcados al empezar la verificación
            foreach (var id in pendientes)
            {
                if (_seleccionados.Remove(id))
                {
                    QuitarDeLista(id);
                    foreach (var fila in Animales.Where(f => f.Animal.Id == id))
                        fila.Seleccionado = false;
                }
            }

            NotificarSeleccion();
        }

        // ===== Auxiliares =====

        // Traduce el origen a los parámetros de la API. Nunca manda incluirInactivos
        private Task<ResultadoApi<ListadoPaginado<AnimalListado>>> ListarAsync(
            OrigenAnimales origen, string? texto, int pagina)
        {
            return _animalService.ListarAsync(
                texto,
                idRodeo: origen.IdRodeo,
                sinRodeo: origen.Tipo == TipoOrigenAnimales.SinRodeo,
                pagina: pagina,
                tamanioPagina: TamanioPagina);
        }

        private void AgregarFilas(IEnumerable<AnimalListado> animales)
        {
            foreach (var animal in animales)
                Animales.Add(new AnimalSeleccionable(animal, _seleccionados.ContainsKey(animal.Id)));
        }

        private void ReemplazarSeleccionado(int id, AnimalListado animal)
        {
            if (!_seleccionados.ContainsKey(id))
                return;

            _seleccionados[id] = animal;

            for (var i = 0; i < _listaSeleccionados.Count; i++)
            {
                if (_listaSeleccionados[i].Id == id)
                {
                    _listaSeleccionados[i] = animal;
                    break;
                }
            }
        }

        private void QuitarDeLista(int id)
        {
            var animal = _listaSeleccionados.FirstOrDefault(a => a.Id == id);
            if (animal is not null)
                _listaSeleccionados.Remove(animal);
        }

        private void MostrarError(string? mensaje, Func<Task> reintentar)
        {
            MensajeError = mensaje;
            _reintentar = reintentar;
        }

        private void ActualizarEstado()
        {
            HayMas = Animales.Count < Total;
            MensajeVacio = HayError ? null : MensajeSinAnimales;
        }

        private void NotificarSeleccion()
        {
            OnPropertyChanged(nameof(CantidadSeleccionados));
            OnPropertyChanged(nameof(TextoSeleccionados));
        }
    }
}
