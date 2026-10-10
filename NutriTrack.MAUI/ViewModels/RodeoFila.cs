using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.ViewModels
{
    // Fila de la lista de rodeos: el rodeo y sus animales al expandirlo.
    // Solo guarda el estado de la fila; los animales los pide RodeosViewModel.
    // Los animales cargados quedan acá mientras la fila exista: la lista se arma de nuevo
    // en cada OnAppearing, así que al volver a la pantalla se piden otra vez
    public partial class RodeoFila : ObservableObject
    {
        public RodeoFila(Rodeo rodeo)
        {
            Rodeo = rodeo;
        }

        public Rodeo Rodeo { get; }

        // Animales cargados hasta ahora (se van sumando con "Cargar más")
        public ObservableCollection<AnimalListado> Animales { get; } = [];

        // Última página pedida a la API
        public int Pagina { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoExpandir))]
        public partial bool Expandido { get; set; }

        // true cuando ya se trajo la primera página: no se vuelve a pedir al contraer y expandir.
        // Si la carga falla queda en false, para reintentar al volver a expandir
        [ObservableProperty]
        public partial bool Cargado { get; set; }

        // Carga de esta fila; no bloquea el resto de la lista
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(NoEstaCargando))]
        public partial bool Cargando { get; set; }

        public bool NoEstaCargando => !Cargando;

        // Total de animales activos del rodeo, según la API
        [ObservableProperty]
        public partial int Total { get; set; }

        // Hay animales cargados: muestra el encabezado de columnas
        [ObservableProperty]
        public partial bool HayAnimales { get; set; }

        // Quedan animales por traer
        [ObservableProperty]
        public partial bool HayMas { get; set; }

        // Error al cargar los animales de esta fila (null si no hay error)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayError))]
        public partial string? MensajeError { get; set; }

        public bool HayError => !string.IsNullOrEmpty(MensajeError);

        // Mensaje cuando el rodeo no tiene animales (null si tiene o todavía no se cargó)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayMensajeVacio))]
        public partial string? MensajeVacio { get; set; }

        public bool HayMensajeVacio => !string.IsNullOrEmpty(MensajeVacio);

        public string TextoExpandir => Expandido ? "Ocultar animales ▴" : "Ver animales ▾";
    }
}
