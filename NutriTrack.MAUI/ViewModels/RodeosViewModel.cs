using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Lista de rodeos y CU8 - Eliminar un rodeo
    public partial class RodeosViewModel : BaseViewModel
    {
        private const string MensajeRodeoNoEncontrado = "No se encontró el rodeo seleccionado";

        private readonly IRodeoService _rodeoService;

        public RodeosViewModel(IRodeoService rodeoService)
        {
            _rodeoService = rodeoService;
        }

        public ObservableCollection<Rodeo> Rodeos { get; } = [];

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
                Rodeos.Add(rodeo);

            MensajeVacio = "No hay rodeos creados";
        });

        // Pendiente: navegar al alta de rodeo cuando exista
        [RelayCommand]
        private void CrearRodeo()
        {
        }

        // Pendiente: navegar a la transferencia de animales cuando exista
        [RelayCommand]
        private void TransferirAnimales()
        {
        }

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

        // E2: el rodeo ya no existe (lo eliminó otro usuario): avisar y recargar
        private async Task AvisarRodeoNoEncontradoAsync()
        {
            await Shell.Current.DisplayAlertAsync("Eliminar rodeo", MensajeRodeoNoEncontrado, "Aceptar");
            await CargarAsync();
        }
    }
}
