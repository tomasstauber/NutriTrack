using CommunityToolkit.Mvvm.ComponentModel;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.ViewModels
{
    // Fila del selector múltiple de animales (#101): el animal y si está marcado.
    // La marca de la fila es solo para mostrar; la selección de verdad la guarda
    // SelectorAnimalesViewModel por id, para que sobreviva a buscar y a cargar más.
    public partial class AnimalSeleccionable : ObservableObject
    {
        public AnimalSeleccionable(AnimalListado animal, bool seleccionado)
        {
            Animal = animal;
            Seleccionado = seleccionado;
        }

        public AnimalListado Animal { get; }

        [ObservableProperty]
        public partial bool Seleccionado { get; set; }
    }
}
