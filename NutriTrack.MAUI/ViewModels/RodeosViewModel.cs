using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Lista de rodeos, CU8 - Eliminar un rodeo y ver los animales de cada rodeo
    public partial class RodeosViewModel : BaseViewModel
    {
        private const string MensajeRodeoNoEncontrado = "No se encontró el rodeo seleccionado";

        // Animales por página al expandir un rodeo (la API acepta hasta 100)
        private const int TamanioPagina = 100;

        private readonly IRodeoService _rodeoService;
        private readonly IAnimalService _animalService;

        public RodeosViewModel(IRodeoService rodeoService, IAnimalService animalService)
        {
            _rodeoService = rodeoService;
            _animalService = animalService;
        }

        // Filas nuevas en cada carga: así se descartan los animales ya traídos
        public ObservableCollection<RodeoFila> Rodeos { get; } = [];

        // Mensaje cuando la lista queda vacía
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _rodeoService.ListarAsync();

            Rodeos.Clear();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                MensajeVacio = null;
                return;
            }

            // Ya llegan ordenados por nombre y solo los activos
            foreach (var rodeo in resultado.Datos ?? [])
                Rodeos.Add(new RodeoFila(rodeo));

            MensajeVacio = "No hay rodeos creados";
        });

        // CU6: formulario de alta de rodeo
        [RelayCommand]
        private Task CrearRodeo() => Shell.Current.GoToAsync(CrearRodeoViewModel.Ruta);

        // CU7: transferir animales entre rodeos
        [RelayCommand]
        private Task TransferirAnimales() => Shell.Current.GoToAsync(TransferirAnimalesViewModel.Ruta);

        [RelayCommand]
        private async Task EliminarAsync(Rodeo rodeo)
        {
            // R2: resumen previo para confirmar
            ResumenEliminacionRodeo? resumen = null;
            var noEncontrado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _rodeoService.ObtenerResumenEliminacionAsync(rodeo.Id);

                if (resultado.Exito)
                    resumen = resultado.Datos;
                else if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                    noEncontrado = true;
                else
                    MensajeError = resultado.MensajeError;
            });

            if (noEncontrado)
            {
                await AvisarRodeoNoEncontradoAsync();
                return;
            }

            if (resumen is null)
                return;

            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Eliminar rodeo",
                $"Vas a eliminar el rodeo \"{resumen.NombreRodeo}\".\n\n" +
                $"Animales: {resumen.CantidadAnimales}\n" +
                $"Planes asignados: {resumen.CantidadPlanes}\n\n" +
                "Los animales quedan sin rodeo. ¿Querés continuar?",
                "Eliminar", "Cancelar");

            // E1: si cancela, no se llama al DELETE
            if (!confirmado)
                return;

            var eliminado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _rodeoService.EliminarAsync(rodeo.Id);

                if (resultado.Exito)
                    eliminado = true;
                else if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                    noEncontrado = true;
                else
                    MensajeError = resultado.MensajeError;
            });

            if (noEncontrado)
            {
                await AvisarRodeoNoEncontradoAsync();
                return;
            }

            if (!eliminado)
                return;

            await Shell.Current.DisplayAlertAsync("Eliminar rodeo", "Rodeo eliminado con éxito.", "Aceptar");
            await CargarAsync();
        }

        // Expande o contrae la fila. Los animales se piden solo la primera vez que se expande.
        // Varias filas pueden cargar a la vez: cada una se cuida con su propio Cargando
        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task AlternarAnimalesAsync(RodeoFila fila)
        {
            fila.Expandido = !fila.Expandido;

            if (!fila.Expandido || fila.Cargado || fila.Cargando)
                return;

            await CargarAnimalesAsync(fila, pagina: 1);
        }

        // Página siguiente, sumada a lo que ya está cargado
        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task CargarMasAnimalesAsync(RodeoFila fila)
        {
            if (fila.Cargando || !fila.HayMas)
                return;

            await CargarAnimalesAsync(fila, fila.Pagina + 1);
        }

        private async Task CargarAnimalesAsync(RodeoFila fila, int pagina)
        {
            try
            {
                fila.Cargando = true;
                fila.MensajeError = null;

                // Sin incluirInactivos: solo vienen los activos
                var resultado = await _animalService.ListarAsync(
                    texto: null, idRodeo: fila.Rodeo.Id, pagina: pagina, tamanioPagina: TamanioPagina);

                // Si falla no se marca como cargada: al volver a expandir se reintenta
                if (!resultado.Exito)
                {
                    fila.MensajeError = resultado.MensajeError;
                    return;
                }

                fila.Pagina = pagina;
                fila.Total = resultado.Datos?.Total ?? 0;
                foreach (var animal in resultado.Datos?.Items ?? [])
                    fila.Animales.Add(animal);

                fila.Cargado = true;
            }
            finally
            {
                fila.Cargando = false;
                ActualizarEstado(fila);
            }
        }

        private static void ActualizarEstado(RodeoFila fila)
        {
            fila.HayAnimales = fila.Animales.Count > 0;
            fila.HayMas = fila.Cargado && fila.Animales.Count < fila.Total;
            fila.MensajeVacio = fila.Cargado && !fila.HayAnimales && !fila.HayError
                ? "Este rodeo no tiene animales"
                : null;
        }

        // E2: el rodeo ya no existe (lo eliminó otro usuario): avisar y recargar
        private async Task AvisarRodeoNoEncontradoAsync()
        {
            await Shell.Current.DisplayAlertAsync("Eliminar rodeo", MensajeRodeoNoEncontrado, "Aceptar");
            await CargarAsync();
        }
    }
}
