using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // Issue #116 - Filtro de período de los reportes (componente, no una página): rodeo, tipo y fechas.
    // Lo usan Inventario (CU17), Fechas importantes (CU18) y Evolución de peso (CU19).
    //
    // Uso desde la pantalla del reporte:
    //   1. Recibir un FiltroPeriodoReporteViewModel por constructor y exponerlo como propiedad (Filtro).
    //   2. Opcional, en el constructor: valores iniciales con Configurar(tipo, fechaReferencia, desde, hasta).
    //   3. Al aparecer: await Filtro.CargarRodeosAsync().
    //   4. Al generar: if (!Filtro.Validar()) return;  y pedir el reporte con Filtro.ArmarFiltro().
    // En la vista: <views:FiltroPeriodoReporteView BindingContext="{Binding Filtro}" />
    // El filtro no dice qué se cuenta en el período (altas, eventos, pesajes): eso lo explica cada reporte.
    public partial class FiltroPeriodoReporteViewModel : BaseViewModel
    {
        // E3, E4 y E5: los mismos textos que devuelve el back
        private const string MensajeFaltaFechaReferencia = "Debe indicar la fecha del período para este tipo de reporte";
        private const string MensajeFaltaRango = "Debe indicar desde y hasta para un reporte personalizado";
        private const string MensajePeriodoInvalido = "El período ingresado no es válido";

        // CU17 R3: días de cada tipo fijo, contados hacia atrás desde la fecha de referencia
        // (los mismos que PeriodoReporteCalculator del back). Solo se usan para el texto de ayuda
        private static readonly Dictionary<string, int> DiasPorTipo = new()
        {
            [TiposReporte.Actual] = 1,
            [TiposReporte.Diario] = 1,
            [TiposReporte.Semanal] = 7,
            [TiposReporte.Quincenal] = 15,
            [TiposReporte.Mensual] = 30,
            [TiposReporte.Anual] = 365
        };

        private static readonly OpcionRodeoReporte OpcionTodos = new() { Texto = "Todos", IdRodeo = null };

        private readonly IRodeoService _rodeoService;

        public FiltroPeriodoReporteViewModel(IRodeoService rodeoService)
        {
            _rodeoService = rodeoService;
        }

        // ===== Rodeo =====

        // "Todos" y los rodeos activos (GET api/Rodeo)
        [ObservableProperty]
        public partial IReadOnlyList<OpcionRodeoReporte> Rodeos { get; set; } = [OpcionTodos];

        [ObservableProperty]
        public partial OpcionRodeoReporte? RodeoSeleccionado { get; set; } = OpcionTodos;

        // ===== Tipo =====

        // D2: los siete tipos que acepta el back
        public IReadOnlyList<OpcionTipoReporte> Tipos { get; } =
        [
            new() { Texto = "Actual", Valor = TiposReporte.Actual },
            new() { Texto = "Diario", Valor = TiposReporte.Diario },
            new() { Texto = "Semanal", Valor = TiposReporte.Semanal },
            new() { Texto = "Quincenal", Valor = TiposReporte.Quincenal },
            new() { Texto = "Mensual", Valor = TiposReporte.Mensual },
            new() { Texto = "Anual", Valor = TiposReporte.Anual },
            new() { Texto = "Personalizado", Valor = TiposReporte.Personalizado }
        ];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarFechaReferencia))]
        [NotifyPropertyChangedFor(nameof(MostrarRango))]
        [NotifyPropertyChangedFor(nameof(TextoPeriodo))]
        [NotifyPropertyChangedFor(nameof(HayTextoPeriodo))]
        public partial OpcionTipoReporte? TipoSeleccionado { get; set; }

        // De diario a anual se pide la fecha de referencia; personalizado pide desde y hasta; actual, nada
        public bool MostrarFechaReferencia => TipoSeleccionado is { } tipo &&
            tipo.Valor != TiposReporte.Actual && tipo.Valor != TiposReporte.Personalizado;

        public bool MostrarRango => TipoSeleccionado?.Valor == TiposReporte.Personalizado;

        // ===== Fechas =====

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoPeriodo))]
        [NotifyPropertyChangedFor(nameof(HayTextoPeriodo))]
        public partial DateTime? FechaReferencia { get; set; } = DateTime.Today;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoPeriodo))]
        [NotifyPropertyChangedFor(nameof(HayTextoPeriodo))]
        public partial DateTime? Desde { get; set; } = DateTime.Today;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoPeriodo))]
        [NotifyPropertyChangedFor(nameof(HayTextoPeriodo))]
        public partial DateTime? Hasta { get; set; } = DateTime.Today;

        // Rango que se va a consultar, con la misma cuenta que el back (CU17 R3),
        // para que se entienda qué abarca cada tipo. null si faltan datos
        public string? TextoPeriodo
        {
            get
            {
                var tipo = TipoSeleccionado?.Valor;

                if (tipo == TiposReporte.Personalizado)
                {
                    return Desde is { } desde && Hasta is { } hasta && desde.Date <= hasta.Date
                        ? $"Período: del {Formatear(desde)} al {Formatear(hasta)}, las dos fechas incluidas."
                        : null;
                }

                if (tipo is null || !DiasPorTipo.TryGetValue(tipo, out var dias))
                    return null;

                if (tipo == TiposReporte.Actual)
                    return $"Período: solo hoy ({Formatear(DateTime.Today)}).";

                if (FechaReferencia is not { } referencia)
                    return null;

                if (dias == 1)
                    return $"Período: solo el {Formatear(referencia)}.";

                var inicio = referencia.Date.AddDays(-(dias - 1));
                return $"Período: del {Formatear(inicio)} al {Formatear(referencia)} " +
                    $"(la fecha elegida y los {dias - 1} días anteriores).";
            }
        }

        public bool HayTextoPeriodo => !string.IsNullOrEmpty(TextoPeriodo);

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para el error al cargar los rodeos

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorTipo))]
        public partial string? ErrorTipo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorFechaReferencia))]
        public partial string? ErrorFechaReferencia { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorDesde))]
        public partial string? ErrorDesde { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorHasta))]
        public partial string? ErrorHasta { get; set; }

        public bool HayErrorTipo => !string.IsNullOrEmpty(ErrorTipo);
        public bool HayErrorFechaReferencia => !string.IsNullOrEmpty(ErrorFechaReferencia);
        public bool HayErrorDesde => !string.IsNullOrEmpty(ErrorDesde);
        public bool HayErrorHasta => !string.IsNullOrEmpty(ErrorHasta);

        // Al cambiar el tipo, los errores de fechas del tipo anterior ya no aplican
        partial void OnTipoSeleccionadoChanged(OpcionTipoReporte? value)
        {
            ErrorTipo = null;
            ErrorFechaReferencia = null;
            ErrorDesde = null;
            ErrorHasta = null;
        }

        // ===== Interfaz pública para la pantalla del reporte =====

        // Valores iniciales del reporte. Las fechas que no se pasan quedan en hoy
        public void Configurar(string tipo, DateTime? fechaReferencia = null, DateTime? desde = null, DateTime? hasta = null)
        {
            TipoSeleccionado = Tipos.FirstOrDefault(t => t.Valor == tipo);
            FechaReferencia = fechaReferencia ?? DateTime.Today;
            Desde = desde ?? DateTime.Today;
            Hasta = hasta ?? DateTime.Today;
        }

        // Al aparecer. Conserva el rodeo elegido (por id) si sigue en la lista; si no, vuelve a "Todos"
        public Task CargarRodeosAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _rodeoService.ListarAsync();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                return;
            }

            var idRodeo = RodeoSeleccionado?.IdRodeo;

            List<OpcionRodeoReporte> opciones = [OpcionTodos];
            opciones.AddRange((resultado.Datos ?? [])
                .Select(r => new OpcionRodeoReporte { Texto = r.Nombre, IdRodeo = r.Id }));

            Rodeos = opciones;
            RodeoSeleccionado = opciones.FirstOrDefault(o => o.IdRodeo == idRodeo) ?? OpcionTodos;
        });

        // Valida los campos que pide el tipo elegido y marca cada uno con su error.
        // true si se puede pedir el reporte
        public bool Validar()
        {
            var tipo = TipoSeleccionado?.Valor;

            ErrorTipo = tipo is null ? "El tipo de reporte es obligatorio." : null;

            // E3: de diario a anual
            ErrorFechaReferencia = MostrarFechaReferencia && FechaReferencia is null
                ? MensajeFaltaFechaReferencia
                : null;

            // E4 y E5: personalizado
            ErrorDesde = MostrarRango && Desde is null ? MensajeFaltaRango : null;
            ErrorHasta = !MostrarRango ? null : Hasta switch
            {
                null => MensajeFaltaRango,
                _ when Desde is { } desde && desde.Date > Hasta.Value.Date => MensajePeriodoInvalido,
                _ => null
            };

            return !HayErrorTipo && !HayErrorFechaReferencia && !HayErrorDesde && !HayErrorHasta;
        }

        // Filtro con solo los datos que corresponden al tipo: el resto queda en null y no viaja.
        // Llamar después de Validar()
        public FiltroReporte ArmarFiltro() => new()
        {
            IdRodeo = RodeoSeleccionado?.IdRodeo,
            TipoReporte = TipoSeleccionado?.Valor ?? string.Empty,
            FechaReferencia = MostrarFechaReferencia ? FechaReferencia?.Date : null,
            Desde = MostrarRango ? Desde?.Date : null,
            Hasta = MostrarRango ? Hasta?.Date : null
        };

        private static string Formatear(DateTime fecha) =>
            fecha.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
    }
}
