using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU6 - Crear un nuevo rodeo con sus animales
    // Se llega desde "Crear rodeo" de la lista de rodeos (solo Encargado y Administrador)
    public partial class CrearRodeoViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "crearRodeo";

        // R2: mínimo de animales del rodeo
        private const int MinimoAnimales = 2;

        private readonly IRodeoService _rodeoService;

        public CrearRodeoViewModel(IRodeoService rodeoService, SelectorAnimalesViewModel selector)
        {
            _rodeoService = rodeoService;
            Selector = selector;
        }

        // Selector múltiple con origen "sin rodeo" (#101)
        public SelectorAnimalesViewModel Selector { get; }

        // Datos del rodeo

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Descripcion { get; set; }

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorNombre))]
        public partial string? ErrorNombre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorAnimales))]
        public partial string? ErrorAnimales { get; set; }

        public bool HayErrorNombre => !string.IsNullOrEmpty(ErrorNombre);
        public bool HayErrorAnimales => !string.IsNullOrEmpty(ErrorAnimales);

        // Al entrar: animales activos sin rodeo.
        // Si se vuelve a llamar, el selector recarga y conserva la selección
        [RelayCommand]
        private Task CargarAsync() => Selector.CambiarOrigenAsync(OrigenAnimales.SinRodeo);

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            // El back no recorta: "Rodeo Norte " sería otro rodeo y "   " pasaría como nombre
            var nombre = Nombre?.Trim() ?? string.Empty;
            var descripcion = Descripcion?.Trim();

            // Se validan los dos campos a la vez, para marcar cada uno con su error

            // R1: nombre no vacío (ya recortado)
            ErrorNombre = string.IsNullOrEmpty(nombre) ? "El nombre del rodeo es obligatorio." : null;

            // R2 / E2: al menos dos animales, antes de llamar a la API
            ErrorAnimales = Selector.CantidadSeleccionados < MinimoAnimales
                ? "Debe seleccionar al menos dos animales."
                : null;

            if (HayErrorNombre || HayErrorAnimales)
                return;

            var pedido = new CrearRodeoRequest
            {
                Nombre = nombre,
                // Vacía viaja en null, no como texto vacío
                Descripcion = string.IsNullOrEmpty(descripcion) ? null : descripcion,
                AnimalesIds = Selector.Seleccionados.Select(a => a.Id).ToList()
            };

            RodeoResponse? creado = null;
            var exito = false;
            var recargarSelector = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _rodeoService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    creado = resultado.Datos;
                    return;
                }

                // E1 (409) y E3 (400): se muestra el mensaje del back y el formulario conserva lo cargado
                MensajeError = resultado.MensajeError;

                // E3: el back avisa "animal no disponible" con un 400. Los otros 400 (nombre vacío,
                // menos de dos animales) ya los frena la validación local, así que se recarga ante
                // cualquier 400: el selector conserva marcados los que siguen sin rodeo
                recargarSelector = resultado.CodigoEstado == HttpStatusCode.BadRequest;
            });

            if (recargarSelector)
            {
                await Selector.RecargarAsync();
                return;
            }

            if (!exito)
                return;

            // S2, S3: lo que devuelve el back (si no llegara la respuesta, lo que se envió)
            var mensaje = string.IsNullOrEmpty(creado?.Mensaje) ? "Rodeo creado con éxito." : creado.Mensaje;
            var nombreRodeo = creado?.NombreRodeo ?? pedido.Nombre;
            var cantidad = creado?.CantidadAnimales ?? pedido.AnimalesIds.Count;

            await Shell.Current.DisplayAlertAsync("Crear rodeo",
                $"{mensaje}\n\nRodeo: {nombreRodeo}\nAnimales asignados: {cantidad}",
                "Aceptar");

            // La lista se recarga al volver (RodeosPage.OnAppearing) y muestra el rodeo nuevo
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
