using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU25 - Consultar ingredientes, CU26 - Editar ingrediente y CU27 - Desactivar ingrediente
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

        // CU24: el catálogo se recarga al volver del alta
        [RelayCommand]
        private Task NuevoIngrediente() =>
            Shell.Current.GoToAsync(NuevoIngredienteViewModel.Ruta);

        // CU26: mismo formulario del alta, precargado con la fila.
        // El catálogo se recarga al volver y muestra la fila actualizada
        [RelayCommand]
        private Task Editar(Ingrediente ingrediente) =>
            Shell.Current.GoToAsync(NuevoIngredienteViewModel.Ruta, new Dictionary<string, object>
            {
                ["ingrediente"] = ingrediente
            });

        // CU27 - Desactivar ingrediente (baja lógica)
        [RelayCommand]
        private async Task DesactivarAsync(Ingrediente ingrediente)
        {
            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Desactivar ingrediente",
                $"Vas a desactivar el ingrediente \"{ingrediente.NombreIngrediente}\". ¿Querés continuar?",
                "Desactivar", "Cancelar");

            // E1: si cancela, no se llama a la API
            if (!confirmado)
                return;

            string? aviso = null;

            await EjecutarAsync(async () =>
            {
                var resultado = await _ingredienteService.DesactivarAsync(ingrediente.Id);

                if (resultado.Exito)
                {
                    // S2: si estaba en uso, el back avisa en qué planes; se muestra tal como llega
                    var texto = resultado.Datos?.Trim().Trim('"');
                    aviso = !string.IsNullOrEmpty(texto) && texto.Contains("planes", StringComparison.OrdinalIgnoreCase)
                        ? $"Ingrediente desactivado con éxito.\n\n{texto}"
                        : "Ingrediente desactivado con éxito.";
                }
                // E3: ya estaba desactivado
                else if (resultado.CodigoEstado == HttpStatusCode.Conflict)
                    aviso = "El ingrediente ya se encuentra desactivado";
                else if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                    aviso = "No existe un ingrediente con ese id";
                else
                    MensajeError = resultado.MensajeError;
            });

            // Error que se queda en pantalla: no se recarga para no borrarlo
            if (aviso is null)
                return;

            await Shell.Current.DisplayAlertAsync("Desactivar ingrediente", aviso, "Aceptar");

            // El ingrediente desaparece del catálogo (solo se listan los activos)
            await CargarAsync();
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
