using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Lista de planes alimenticios: punto de entrada a CU9, CU10 y CU11
    public partial class PlanesAlimenticiosViewModel : BaseViewModel
    {
        private readonly IPlanAlimenticioService _planAlimenticioService;

        public PlanesAlimenticiosViewModel(IPlanAlimenticioService planAlimenticioService)
        {
            _planAlimenticioService = planAlimenticioService;
        }

        public ObservableCollection<PlanAlimenticio> Planes { get; } = [];

        // Mensaje cuando la lista queda vacía
        [ObservableProperty]
        public partial string? MensajeVacio { get; set; }

        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _planAlimenticioService.ListarAsync();

            Planes.Clear();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                MensajeVacio = null;
                return;
            }

            foreach (var plan in resultado.Datos ?? [])
                Planes.Add(plan);

            MensajeVacio = "No hay planes alimenticios creados";
        });

        // CU9: crear plan
        // TODO #114: navegar al formulario de alta del plan
        [RelayCommand]
        private void NuevoPlan()
        {
        }

        // CU11: asignar el plan a un rodeo
        // TODO #115: navegar a la asignación pasando el plan
        [RelayCommand]
        private void AsignarARodeo(PlanAlimenticio plan)
        {
        }
    }
}
