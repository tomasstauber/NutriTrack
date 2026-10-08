using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU10 - Asignar plan alimenticio a un rodeo
    // Se llega desde "Asignar a rodeo" de un plan de la lista, con el plan ya elegido
    public partial class AsignarPlanRodeoViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "asignarPlanRodeo";

        // R2 / E2: texto del issue (el back, si llega a validarlo, manda el suyo)
        private const string MensajeRodeoSinAnimales = "El rodeo seleccionado no existe o no tiene animales asignados.";

        private readonly IPlanAlimenticioService _planAlimenticioService;
        private readonly IRodeoService _rodeoService;

        // true mientras se reemplaza la lista del Picker: al cambiar el ItemsSource,
        // el Picker mueve o borra su selección y eso no tiene que tomarse como un cambio del usuario
        private bool _ajustandoPicker;

        public AsignarPlanRodeoViewModel(IPlanAlimenticioService planAlimenticioService, IRodeoService rodeoService)
        {
            _planAlimenticioService = planAlimenticioService;
            _rodeoService = rodeoService;
        }

        // ===== Plan =====

        // Llega por navegación desde la lista (no hay GET de un plan por id)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayPlan))]
        public partial PlanAlimenticio? Plan { get; set; }

        public bool HayPlan => Plan is not null;

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("plan", out var plan))
                Plan = plan as PlanAlimenticio;
        }

        // ===== Rodeo =====

        // Solo rodeos activos (GET api/Rodeo). CantidadAnimales cuenta solo los activos
        [ObservableProperty]
        public partial IReadOnlyList<Rodeo> Rodeos { get; set; } = [];

        [ObservableProperty]
        public partial Rodeo? RodeoSeleccionado { get; set; }

        // ===== Vigencia =====

        // R3: obligatoria, propone hoy
        [ObservableProperty]
        public partial DateTime? VigenciaDesde { get; set; } = DateTime.Today;

        // Opcional: un DatePicker siempre tiene fecha, así que tiene un CheckBox. Sin tildar, viaja en null
        [ObservableProperty]
        public partial bool CargarVigenciaHasta { get; set; }

        [ObservableProperty]
        public partial DateTime? VigenciaHasta { get; set; }

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorRodeo))]
        public partial string? ErrorRodeo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorVigenciaDesde))]
        public partial string? ErrorVigenciaDesde { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorVigenciaHasta))]
        public partial string? ErrorVigenciaHasta { get; set; }

        public bool HayErrorRodeo => !string.IsNullOrEmpty(ErrorRodeo);
        public bool HayErrorVigenciaDesde => !string.IsNullOrEmpty(ErrorVigenciaDesde);
        public bool HayErrorVigenciaHasta => !string.IsNullOrEmpty(ErrorVigenciaHasta);

        // R2: el aviso aparece al elegir un rodeo vacío, sin esperar a "Asignar"
        partial void OnRodeoSeleccionadoChanged(Rodeo? value)
        {
            if (_ajustandoPicker)
                return;

            ErrorRodeo = value is { CantidadAnimales: <= 0 } ? MensajeRodeoSinAnimales : null;
        }

        // Al tildar la vigencia hasta, arranca en la vigencia desde (cumple R4)
        partial void OnCargarVigenciaHastaChanged(bool value)
        {
            if (value)
                VigenciaHasta = VigenciaDesde ?? DateTime.Today;
        }

        // ===== Carga =====

        // Al aparecer. Conserva el rodeo elegido (por id) si sigue en la lista
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            if (Plan is null)
            {
                MensajeError = "No se indicó el plan alimenticio.";
                return;
            }

            var resultado = await _rodeoService.ListarAsync();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                return;
            }

            var idRodeo = RodeoSeleccionado?.Id;
            var rodeos = resultado.Datos ?? [];

            _ajustandoPicker = true;
            try
            {
                Rodeos = rodeos;
                RodeoSeleccionado = rodeos.FirstOrDefault(r => r.Id == idRodeo);
            }
            finally
            {
                _ajustandoPicker = false;
            }

            // Con la cantidad al día, el aviso de R2 se vuelve a calcular
            ErrorRodeo = RodeoSeleccionado is { CantidadAnimales: <= 0 } ? MensajeRodeoSinAnimales : null;
        });

        // ===== Asignar =====

        [RelayCommand]
        private async Task AsignarAsync()
        {
            MensajeError = null;

            if (Plan is not { } plan)
            {
                MensajeError = "No se indicó el plan alimenticio.";
                return;
            }

            // Se validan los campos a la vez, para marcar cada uno con su error
            var rodeo = RodeoSeleccionado;

            // R2 / E2: rodeo obligatorio y con animales, antes de llamar a la API
            ErrorRodeo = rodeo switch
            {
                null => "El rodeo es obligatorio.",
                { CantidadAnimales: <= 0 } => MensajeRodeoSinAnimales,
                _ => null
            };

            var desde = VigenciaDesde?.Date;
            ErrorVigenciaDesde = desde is null ? "La vigencia desde es obligatoria." : null;

            // R4: si se carga, igual o posterior a la vigencia desde
            ErrorVigenciaHasta = ValidarVigenciaHasta(desde);

            if (HayErrorRodeo || HayErrorVigenciaDesde || HayErrorVigenciaHasta)
                return;

            var pedido = new AsignarPlanRequest
            {
                IdPlanAlimenticio = plan.Id,
                IdRodeo = rodeo!.Id,
                VigenciaDesde = DateOnly.FromDateTime(desde!.Value),
                VigenciaHasta = CargarVigenciaHasta && VigenciaHasta is { } hasta
                    ? DateOnly.FromDateTime(hasta)
                    : null
            };

            AsignarPlanResponse? asignacion = null;
            var exito = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _planAlimenticioService.AsignarARodeoAsync(pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    asignacion = resultado.Datos;
                    return;
                }

                // E1, E2 y E4: se muestra el mensaje del back y el formulario conserva lo cargado
                MensajeError = resultado.MensajeError;
            });

            if (!exito)
                return;

            await Shell.Current.DisplayAlertAsync("Asignar plan a rodeo",
                ArmarResumen(asignacion, pedido, plan, rodeo),
                "Aceptar");

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        private string? ValidarVigenciaHasta(DateTime? desde)
        {
            if (!CargarVigenciaHasta)
                return null;

            if (VigenciaHasta is null)
                return "Elegí la fecha o destildá la opción.";

            return desde is not null && VigenciaHasta.Value.Date < desde
                ? "La vigencia hasta debe ser igual o posterior a la vigencia desde."
                : null;
        }

        // S2 a S8: lo que confirma el back (si no llegara la respuesta, lo que se envió).
        // Cantidad y kg totales solo vienen del back (R5)
        private static string ArmarResumen(AsignarPlanResponse? asignacion, AsignarPlanRequest pedido,
            PlanAlimenticio plan, Rodeo rodeo)
        {
            var nombrePlan = string.IsNullOrEmpty(asignacion?.NombrePlan) ? plan.NombrePlan : asignacion.NombrePlan;
            var nombreRodeo = string.IsNullOrEmpty(asignacion?.NombreRodeo) ? rodeo.Nombre : asignacion.NombreRodeo;
            var desde = asignacion?.VigenciaDesde ?? pedido.VigenciaDesde;
            var hasta = asignacion is null ? pedido.VigenciaHasta : asignacion.VigenciaHasta;

            var resumen = "Plan alimenticio asignado con éxito.\n\n" +
                $"Plan: {nombrePlan}\n" +
                $"Rodeo: {nombreRodeo}\n" +
                $"Vigencia desde: {FormatearFecha(desde)}\n" +
                $"Vigencia hasta: {(hasta is { } fin ? FormatearFecha(fin) : "Sin fecha de fin")}";

            if (asignacion is not null)
            {
                resumen += $"\nAnimales: {asignacion.CantidadAnimales}" +
                    $"\nKg MS diaria total: {asignacion.KgMsDiariaTotal:0.##}";

                // R7 / S8: solo si el rodeo tenía otro plan activo
                if (!string.IsNullOrEmpty(asignacion.PlanAnteriorReemplazado))
                    resumen += $"\n\nReemplazó al plan: {asignacion.PlanAnteriorReemplazado}";
            }

            return resumen;
        }

        private static string FormatearFecha(DateOnly fecha) =>
            fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }
}
