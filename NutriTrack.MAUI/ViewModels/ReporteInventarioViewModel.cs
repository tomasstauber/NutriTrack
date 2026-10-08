using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU17 - Generar reporte de inventario de animales
    // Se llega desde "Inventario de animales" del menú de reportes (solo Administrador)
    public partial class ReporteInventarioViewModel : BaseViewModel
    {
        // Ruta de Shell del reporte (se registra en AppShell.xaml.cs)
        public const string Ruta = "reporteInventario";

        // Si el back no manda el nombre del PDF
        private const string NombrePdfPorDefecto = "inventario_animales.pdf";

        private readonly IReporteService _reporteService;
        private readonly IArchivoService _archivoService;

        public ReporteInventarioViewModel(IReporteService reporteService, IArchivoService archivoService,
            FiltroPeriodoReporteViewModel filtro)
        {
            _reporteService = reporteService;
            _archivoService = archivoService;
            Filtro = filtro;

            // Abre en mensual con la fecha de hoy: altas de los últimos 30 días
            Filtro.Configurar(TiposReporte.Mensual);
        }

        // Filtro de período compartido con los otros reportes
        public FiltroPeriodoReporteViewModel Filtro { get; }

        // ===== Resultado =====

        // Ordenados por caravana: el back no los ordena
        public ObservableCollection<AnimalInventario> Animales { get; } = [];

        // S1: "Total de animales: N" (null hasta que hay resultado)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayResultado))]
        public partial string? TextoTotal { get; set; }

        public bool HayResultado => !string.IsNullOrEmpty(TextoTotal);

        // E6 / E1: el 404 se muestra como estado vacío con el mensaje del back, no como error
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayMensajeVacio))]
        public partial string? MensajeVacio { get; set; }

        public bool HayMensajeVacio => !string.IsNullOrEmpty(MensajeVacio);

        // true mientras se descarga el PDF, para mostrar "Descargando PDF…"
        [ObservableProperty]
        public partial bool DescargandoPdf { get; set; }

        // Al aparecer: rodeos del filtro (conserva el elegido)
        [RelayCommand]
        private Task CargarAsync() => Filtro.CargarRodeosAsync();

        [RelayCommand]
        private async Task GenerarAsync()
        {
            MensajeError = null;

            // E3, E4 y E5: los marca el filtro en sus campos, sin llamar a la API
            if (!Filtro.Validar())
                return;

            var filtro = Filtro.ArmarFiltro();

            await EjecutarAsync(async () =>
            {
                // El resultado anterior no corresponde al filtro nuevo
                Animales.Clear();
                TextoTotal = null;
                MensajeVacio = null;

                var resultado = await _reporteService.InventarioAnimalesAsync(filtro);

                if (!resultado.Exito)
                {
                    // Cualquier 404 de este endpoint (sin resultados o rodeo inexistente) es estado vacío
                    if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                        MensajeVacio = resultado.MensajeError;
                    else
                        MensajeError = resultado.MensajeError;
                    return;
                }

                var animales = resultado.Datos?.Animales ?? [];

                foreach (var animal in animales.OrderBy(a => a.Caravana, StringComparer.OrdinalIgnoreCase))
                    Animales.Add(animal);

                TextoTotal = $"Total de animales: {resultado.Datos?.TotalAnimales ?? animales.Count}";
            });
        }

        // Mismo filtro y mismas validaciones que Generar. No toca la tabla en pantalla
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
                    var resultado = await _reporteService.InventarioAnimalesPdfAsync(filtro);

                    // 404 (sin resultados, rodeo inexistente) y el resto: mensaje del back, sin abrir nada
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
