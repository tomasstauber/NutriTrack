using System.Collections.ObjectModel;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU19 - Generar reporte de evolución de peso
    // Se llega desde "Evolución de peso" del menú de reportes (solo Administrador)
    public partial class ReporteEvolucionPesoViewModel : BaseViewModel
    {
        // Ruta de Shell del reporte (se registra en AppShell.xaml.cs)
        public const string Ruta = "reporteEvolucionPeso";

        // Si el back no manda el nombre del PDF
        private const string NombrePdfPorDefecto = "evolucion_peso.pdf";

        private const string MensajeCaravanaIncompleta =
            "La caravana está incompleta: informe el CUIG y el número de manejo.";

        private readonly IReporteService _reporteService;
        private readonly IArchivoService _archivoService;

        // El reporte se genera solo la primera vez que aparece la pantalla
        private bool _generadoAlAbrir;

        // Filtro con el que se generó la tabla en pantalla: el detalle (S2) lo repite
        // aunque después se cambien los campos sin volver a generar
        private FiltroReporte? _filtroGenerado;

        public ReporteEvolucionPesoViewModel(IReporteService reporteService, IArchivoService archivoService,
            FiltroPeriodoReporteViewModel filtro)
        {
            _reporteService = reporteService;
            _archivoService = archivoService;
            Filtro = filtro;

            // Este reporte mira hacia atrás: abre en anual con la fecha de hoy (los últimos 365 días),
            // así la mayoría de los animales tiene más de un pesaje y se ve la variación
            Filtro.Configurar(TiposReporte.Anual);
        }

        // Filtro de período compartido con los otros reportes
        public FiltroPeriodoReporteViewModel Filtro { get; }

        // ===== Filtro de caravana (D4, opcional) =====

        [ObservableProperty]
        public partial string? Cuig { get; set; }

        [ObservableProperty]
        public partial string? NroManejo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorCaravana))]
        public partial string? ErrorCaravana { get; set; }

        public bool HayErrorCaravana => !string.IsNullOrEmpty(ErrorCaravana);

        // ===== Resultado (S1) =====

        // Ya vienen ordenados por caravana
        public ObservableCollection<AnimalEvolucionPeso> Animales { get; } = [];

        [ObservableProperty]
        public partial bool HayResultado { get; set; }

        // E1: el 404 se muestra como estado vacío con el mensaje del back, no como error
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayMensajeVacio))]
        public partial string? MensajeVacio { get; set; }

        public bool HayMensajeVacio => !string.IsNullOrEmpty(MensajeVacio);

        // ===== Detalle de pesajes (S2) =====

        public ObservableCollection<PesajeDetalle> Pesajes { get; } = [];

        // true mientras la sección del detalle está a la vista (con pesajes o con su error)
        [ObservableProperty]
        public partial bool HayDetalle { get; set; }

        [ObservableProperty]
        public partial string? TituloDetalle { get; set; }

        [ObservableProperty]
        public partial bool HayPesajes { get; set; }

        // Error del pedido del detalle: va en su sección y no borra la tabla
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorDetalle))]
        public partial string? ErrorDetalle { get; set; }

        public bool HayErrorDetalle => !string.IsNullOrEmpty(ErrorDetalle);

        // true mientras se descarga el PDF, para mostrar "Descargando PDF…"
        [ObservableProperty]
        public partial bool DescargandoPdf { get; set; }

        // Al aparecer: rodeos del filtro (conserva el elegido) y, la primera vez,
        // el reporte con los valores por defecto (el último año)
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

            // E2 y caravana: los marca cada campo, sin llamar a la API.
            // Se validan los dos para mostrar todos los errores juntos
            var periodoValido = Filtro.Validar();
            var caravanaValida = ValidarCaravana(out var caravana);
            if (!periodoValido || !caravanaValida)
                return;

            var filtro = Filtro.ArmarFiltro();

            await EjecutarAsync(async () =>
            {
                // El resultado anterior no corresponde al filtro nuevo
                Animales.Clear();
                HayResultado = false;
                MensajeVacio = null;
                LimpiarDetalle();
                _filtroGenerado = null;

                var resultado = await _reporteService.EvolucionPesoAsync(filtro, caravana);

                if (!resultado.Exito)
                {
                    // Cualquier 404 de este endpoint (sin registros, rodeo o caravana inexistente)
                    // es estado vacío con el mensaje del back
                    if (resultado.CodigoEstado == HttpStatusCode.NotFound)
                        MensajeVacio = resultado.MensajeError;
                    else
                        MensajeError = resultado.MensajeError;
                    return;
                }

                foreach (var animal in resultado.Datos?.Animales ?? [])
                    Animales.Add(animal);

                HayResultado = true;
                _filtroGenerado = filtro;

                // Filtrado por caravana: el back ya manda el detalle de ese animal
                if (caravana is not null && resultado.Datos?.Detalle is { } detalle)
                    MostrarDetalle(caravana, detalle);
            });
        }

        // S2: al tocar una fila, el mismo reporte (período y rodeo de la tabla) con la caravana de la fila
        [RelayCommand]
        private async Task VerDetalleAsync(AnimalEvolucionPeso? animal)
        {
            if (animal is null || _filtroGenerado is not { } filtro)
                return;

            await EjecutarAsync(async () =>
            {
                LimpiarDetalle();
                TituloDetalle = $"Pesajes de {animal.Caravana}";
                HayDetalle = true;

                // La caravana tal como la devolvió el back ("CUIG-NRO")
                var resultado = await _reporteService.EvolucionPesoAsync(filtro, animal.Caravana);

                // Cualquier falla queda en la sección del detalle: la tabla sigue a la vista
                if (!resultado.Exito)
                {
                    ErrorDetalle = resultado.MensajeError;
                    return;
                }

                MostrarDetalle(animal.Caravana, resultado.Datos?.Detalle);
            });
        }

        // Mismos filtros y mismas validaciones que Generar (caravana incluida). No toca la tabla en pantalla
        [RelayCommand]
        private async Task DescargarPdfAsync()
        {
            MensajeError = null;

            var periodoValido = Filtro.Validar();
            var caravanaValida = ValidarCaravana(out var caravana);
            if (!periodoValido || !caravanaValida)
                return;

            var filtro = Filtro.ArmarFiltro();
            string? mensajeFalla = null;

            // EjecutarAsync deshabilita Generar y Descargar mientras dura: no se dispara dos veces
            await EjecutarAsync(async () =>
            {
                DescargandoPdf = true;
                try
                {
                    var resultado = await _reporteService.EvolucionPesoPdfAsync(filtro, caravana);

                    // 404 (sin registros, rodeo o caravana inexistente) y el resto: mensaje del back, sin abrir nada
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

        // D4: las dos partes o ninguna. Se recortan y pasan a mayúsculas como en el alta de animal.
        // caravana queda en "CUIG-NRO", o null si no se informó
        private bool ValidarCaravana(out string? caravana)
        {
            caravana = null;

            var cuig = AnimalValidador.NormalizarCaravana(Cuig);
            var nroManejo = AnimalValidador.NormalizarCaravana(NroManejo);

            if (string.IsNullOrEmpty(cuig) && string.IsNullOrEmpty(nroManejo))
            {
                ErrorCaravana = null;
                return true;
            }

            if (string.IsNullOrEmpty(cuig) || string.IsNullOrEmpty(nroManejo))
                ErrorCaravana = MensajeCaravanaIncompleta;
            else if (!CaravanaValidador.EsValida(cuig, nroManejo))
                ErrorCaravana = CaravanaValidador.MensajeFormatoInvalido;
            else
                ErrorCaravana = null;

            if (HayErrorCaravana)
                return false;

            caravana = CaravanaValidador.Formatear(cuig, nroManejo);
            return true;
        }

        private void MostrarDetalle(string caravana, DetallePesajes? detalle)
        {
            Pesajes.Clear();
            foreach (var pesaje in detalle?.Pesajes ?? [])
                Pesajes.Add(pesaje);

            TituloDetalle = $"Pesajes de {caravana} ({Pesajes.Count})";
            HayPesajes = Pesajes.Count > 0;
            ErrorDetalle = null;
            HayDetalle = true;
        }

        private void LimpiarDetalle()
        {
            Pesajes.Clear();
            HayDetalle = false;
            HayPesajes = false;
            TituloDetalle = null;
            ErrorDetalle = null;
        }
    }
}
