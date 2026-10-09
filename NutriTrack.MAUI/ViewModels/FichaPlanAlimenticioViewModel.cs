using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Issue #159 - Ficha del plan alimenticio (no tiene CU propio: consulta del plan,
    // punto de entrada a CU10 y CU11). Recibe por navegación "idPlan"
    public partial class FichaPlanAlimenticioViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell de la ficha (se registra en AppShell.xaml.cs)
        public const string Ruta = "ficha-plan-alimenticio";

        private readonly IPlanAlimenticioService _planAlimenticioService;

        // Dato recibido por navegación
        private int? _idPlan;

        public FichaPlanAlimenticioViewModel(IPlanAlimenticioService planAlimenticioService)
        {
            _planAlimenticioService = planAlimenticioService;
        }

        // Plan con sus componentes (null hasta cargarlo o si falló)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayPlan))]
        [NotifyPropertyChangedFor(nameof(TextoSuma))]
        public partial PlanAlimenticioCompleto? Plan { get; set; }

        public bool HayPlan => Plan is not null;

        // Suma de los porcentajes de inclusión, con decimal
        public string TextoSuma => Plan is null
            ? string.Empty
            : $"Suma de porcentajes: {Plan.Detalles.Sum(d => d.PorcentajeInclusionMs):0.##}%";

        // Asignaciones activas del plan
        public ObservableCollection<AsignacionActivaPlan> Asignaciones { get; } = [];

        // Error de las asignaciones, aparte del de la ficha: si fallan, el plan igual se ve
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorAsignaciones))]
        public partial string? MensajeErrorAsignaciones { get; set; }

        public bool HayErrorAsignaciones => !string.IsNullOrEmpty(MensajeErrorAsignaciones);

        // "El plan no está asignado a ningún rodeo": solo con una lista vacía que llegó bien
        [ObservableProperty]
        public partial bool SinAsignaciones { get; set; }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("idPlan", out var valor) && valor is int idPlan)
                _idPlan = idPlan;
        }

        // Al aparecer: plan y asignaciones, cada uno con su propio error.
        // Al volver de asignar o editar se ven los cambios
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            if (_idPlan is null)
            {
                Plan = null;
                MensajeError = "No se indicó el plan.";
                return;
            }

            var plan = await _planAlimenticioService.ObtenerAsync(_idPlan.Value);

            if (!plan.Exito)
            {
                // 404: "No existe un plan con ese Id."
                Plan = null;
                MensajeError = plan.MensajeError;
                return;
            }

            Plan = plan.Datos;

            await CargarAsignacionesAsync(_idPlan.Value);
        });

        private async Task CargarAsignacionesAsync(int idPlan)
        {
            Asignaciones.Clear();
            MensajeErrorAsignaciones = null;
            SinAsignaciones = false;

            var resultado = await _planAlimenticioService.ListarAsignacionesActivasAsync(idPlan);

            if (!resultado.Exito)
            {
                MensajeErrorAsignaciones = resultado.MensajeError;
                return;
            }

            foreach (var asignacion in resultado.Datos ?? [])
                Asignaciones.Add(asignacion);

            SinAsignaciones = Asignaciones.Count == 0;
        }

        // Rodeo asignado: se pasa la asignación completa (no hay GET de un rodeo por id)
        [RelayCommand]
        private Task VerRodeo(AsignacionActivaPlan asignacion) =>
            Shell.Current.GoToAsync(RodeoAsignadoViewModel.Ruta, new Dictionary<string, object>
            {
                ["asignacion"] = asignacion
            });

        // CU10: la pantalla de asignar recibe el plan como lo pasa la lista
        [RelayCommand]
        private Task AsignarARodeo()
        {
            if (Plan is null)
                return Task.CompletedTask;

            var plan = new PlanAlimenticio
            {
                Id = Plan.Id,
                NombrePlan = Plan.NombrePlan,
                Categoria = Plan.Categoria,
                TipoAlimentacion = Plan.TipoAlimentacion,
                KgMsDiariaPorAnimal = Plan.KgMsDiariaPorAnimal
            };

            return Shell.Current.GoToAsync(AsignarPlanRodeoViewModel.Ruta, new Dictionary<string, object>
            {
                ["plan"] = plan
            });
        }

        // CU11: mismo formulario del alta en modo edición
        [RelayCommand]
        private Task Editar()
        {
            if (Plan is null)
                return Task.CompletedTask;

            return Shell.Current.GoToAsync(NuevoPlanAlimenticioViewModel.Ruta, new Dictionary<string, object>
            {
                ["idPlan"] = Plan.Id
            });
        }
    }
}
