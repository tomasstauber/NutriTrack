using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU18 - Generar reporte de fechas importantes a recordar
    // Se llega desde "Fechas importantes" del menú de reportes (solo Administrador)
    public partial class ReporteFechasImportantesViewModel : BaseViewModel
    {
        // Ruta de Shell del reporte (se registra en AppShell.xaml.cs)
        public const string Ruta = "reporteFechasImportantes";

        // Días hacia adelante del período inicial
        private const int DiasPorDefecto = 7;

        // Si el back no manda el nombre del PDF
        private const string NombrePdfPorDefecto = "fechas_importantes.pdf";

        private readonly IReporteService _reporteService;
        private readonly IArchivoService _archivoService;

        // El reporte se genera solo la primera vez que aparece la pantalla
        private bool _generadoAlAbrir;

        public ReporteFechasImportantesViewModel(IReporteService reporteService, IArchivoService archivoService,
            FiltroPeriodoReporteViewModel filtro)
        {
            _reporteService = reporteService;
            _archivoService = archivoService;
            Filtro = filtro;

            // Este reporte mira hacia adelante y los tipos fijos cuentan hacia atrás (CU17 R3):
            // abre en personalizado, de hoy a hoy + 7 días
            Filtro.Configurar(TiposReporte.Personalizado,
                desde: DateTime.Today,
                hasta: DateTime.Today.AddDays(DiasPorDefecto));
        }

        // Filtro de período compartido con los otros reportes
        public FiltroPeriodoReporteViewModel Filtro { get; }

        // ===== Resultado =====

        // S1 y S2, ordenadas por fecha y, a igual fecha, por destino: el back no las ordena
        public ObservableCollection<ProximaAplicacion> ProximasAplicaciones { get; } = [];
        public ObservableCollection<VencimientoEvento> Vencimientos { get; } = [];

        // true cuando llegó un 200 (aunque una de las dos listas venga vacía)
        [ObservableProperty]
        public partial bool HayResultado { get; set; }

        // Títulos de las secciones con la cantidad de filas
        [ObservableProperty]
        public partial string? TituloProximas { get; set; }

        [ObservableProperty]
        public partial string? TituloVencimientos { get; set; }

        // Una sección puede venir vacía y la otra no: se muestra un texto en lugar de la tabla
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SinProximas))]
        public partial bool HayProximas { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(SinVencimientos))]
        public partial bool HayVencimientos { get; set; }

        public bool SinProximas => !HayProximas;
        public bool SinVencimientos => !HayVencimientos;

        // E1: el 404 se muestra como estado vacío con el mensaje del back, no como error
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayMensajeVacio))]
        public partial string? MensajeVacio { get; set; }

        public bool HayMensajeVacio => !string.IsNullOrEmpty(MensajeVacio);

        // true mientras se descarga el PDF, para mostrar "Descargando PDF…"
        [ObservableProperty]
        public partial bool DescargandoPdf { get; set; }

        // Al aparecer: rodeos del filtro (conserva el elegido) y, la primera vez,
        // el reporte con los valores por defecto (los próximos 7 días)
        [RelayCommand]
        private async Task CargarAsync()
        {
            await Filtro.CargarRodeosAsync();

            if (_generadoAlAbrir)
                return;

            _generadoAlAbrir = true;
            await GenerarAsync();
        }

        [RelayCommand]
        private async Task GenerarAsync()
        {
            MensajeError = null;

            // E2 (período inválido o incompleto): lo marca el filtro en sus campos, sin llamar a la API
            if (!Filtro.Validar())
                return;

            var filtro = Filtro.ArmarFiltro();

            await EjecutarAsync(async () =>
            {
                // El resultado anterior no corresponde al filtro nuevo
                ProximasAplicaciones.Clear();
                Vencimientos.Clear();
                HayResultado = false;
                MensajeVacio = null;

                var resultado = await _reporteService.FechasImportantesAsync(filtro);

                if (!resultado.Exito)
                {
                    // Cualquier 404 de este endpoint (E1 sin fechas, E3 rodeo inexistente)
                    // es estado vacío con el mensaje del back
                    if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                        MensajeVacio = resultado.MensajeError;
                    else
                        MensajeError = resultado.MensajeError;
                    return;
                }

                var proximas = resultado.Datos?.ProximasAplicaciones ?? [];
                var vencimientos = resultado.Datos?.VencimientosEventos ?? [];

                foreach (var proxima in proximas
                    .OrderBy(p => p.FechaProximaAplicacion.Date)
                    .ThenBy(p => p.Destino, StringComparer.OrdinalIgnoreCase))
                    ProximasAplicaciones.Add(proxima);

                foreach (var vencimiento in vencimientos
                    .OrderBy(v => v.VigenciaHasta.Date)
                    .ThenBy(v => v.Destino, StringComparer.OrdinalIgnoreCase))
                    Vencimientos.Add(vencimiento);

                TituloProximas = $"Próximas aplicaciones ({ProximasAplicaciones.Count})";
                TituloVencimientos = $"Vencimientos ({Vencimientos.Count})";
                HayProximas = ProximasAplicaciones.Count > 0;
                HayVencimientos = Vencimientos.Count > 0;
                HayResultado = true;
            });
        }

        // Mismo filtro y mismas validaciones que Generar. No toca las tablas en pantalla
        [RelayCommand]
        private async Task DescargarPdfAsync()
        {
            MensajeError = null;

            if (!Filtro.Validar())
                return;

            var filtro = Filtro.ArmarFiltro();
            string? mensajeFalla = null;

            // EjecutarAsync deshabilita Generar y Descargar mientras dura: no se dispara dos veces
            await EjecutarAsync(async () =>
            {
                DescargandoPdf = true;
                try
                {
                    var resultado = await _reporteService.FechasImportantesPdfAsync(filtro);

                    // 404 (sin fechas, rodeo inexistente) y el resto: mensaje del back, sin abrir nada
                    if (!resultado.Exito || resultado.Datos is not { } archivo)
                    {
                        mensajeFalla = resultado.MensajeError ?? "No se pudo descargar el PDF.";
                        return;
                    }

                    var guardado = await _archivoService.GuardarYAbrirAsync(archivo, NombrePdfPorDefecto);

                    if (!guardado.Exito)
                        mensajeFalla = guardado.MensajeError;
                }
                finally
                {
                    DescargandoPdf = false;
                }
            });

            if (mensajeFalla is not null)
                await Shell.Current.DisplayAlertAsync("Descargar PDF", mensajeFalla, "Aceptar");
        }
    }
}
