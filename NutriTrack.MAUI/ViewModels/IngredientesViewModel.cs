using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU25 - Consultar ingredientes
    public partial class IngredientesViewModel : BaseViewModel
    {
        private readonly IIngredienteService _ingredienteService;

        // Catálogo completo tal como vino de la API; la búsqueda filtra sobre esta lista
        private List<Ingrediente> _todos = [];

        public IngredientesViewModel(IIngredienteService ingredienteService)
        {
            _ingredienteService = ingredienteService;
        }

        // Lo que se muestra en la tabla (catálogo filtrado por la búsqueda)
        public ObservableCollection<Ingrediente> Ingredientes { get; } = [];

        [ObservableProperty]
        public partial string? TextoBusqueda { get; set; }

        // Mensaje cuando la tabla queda vacía (E1 o E2)
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        partial void OnTextoBusquedaChanged(string? value) => Filtrar();

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _ingredienteService.ListarAsync();

            if (resultado.Exito)
            {
                _todos = resultado.Datos ?? [];
            }
            else
            {
                _todos = [];
                MensajeError = resultado.MensajeError;
            }

            Filtrar();
        });

        // Pendiente: navegar al formulario de alta cuando exista
        [RelayCommand]
        private void NuevoIngrediente()
        {
        }

        // R2: búsqueda por nombre en memoria, parcial y sin distinguir mayúsculas
        private void Filtrar()
        {
            var texto = TextoBusqueda?.Trim();

            var filtrados = string.IsNullOrEmpty(texto)
                ? _todos
                : _todos.Where(i => i.NombreIngrediente.Contains(texto, StringComparison.CurrentCultureIgnoreCase)).ToList();

            Ingredientes.Clear();
            foreach (var ingrediente in filtrados)
                Ingredientes.Add(ingrediente);

            if (HayError)
                MensajeVacio = null;
            else if (_todos.Count == 0)
                MensajeVacio = "No hay ingredientes cargados.";
            else
                MensajeVacio = "No existe un ingrediente con ese nombre.";
        }
    }
}
