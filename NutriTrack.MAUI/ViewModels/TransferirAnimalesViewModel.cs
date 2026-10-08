using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU7 - Transferir animales entre rodeos
    // Se llega desde "Transferir animales" de la lista de rodeos
    public partial class TransferirAnimalesViewModel : BaseViewModel
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "transferirAnimales";

        private const string MensajeOrigenSinAnimales = "El rodeo origen no tiene animales activos.";

        private readonly IRodeoService _rodeoService;

        // true mientras se reemplazan las listas de los Picker: al cambiar el ItemsSource,
        // el Picker mueve o borra su selección y eso no tiene que tomarse como un cambio del usuario
        private bool _ajustandoPickers;

        public TransferirAnimalesViewModel(IRodeoService rodeoService, SelectorAnimalesViewModel selector)
        {
            _rodeoService = rodeoService;
            Selector = selector;
        }

        // Selector múltiple con origen "de un rodeo" (#101)
        public SelectorAnimalesViewModel Selector { get; }

        // ===== Rodeos =====

        // Solo rodeos activos (GET api/Rodeo). CantidadAnimales cuenta solo los activos
        [ObservableProperty]
        public partial IReadOnlyList<Rodeo> Rodeos { get; set; } = [];

        // R3: los mismos rodeos sin el origen
        [ObservableProperty]
        public partial IReadOnlyList<Rodeo> RodeosDestino { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarSelector))]
        [NotifyPropertyChangedFor(nameof(MostrarElegirOrigen))]
        public partial Rodeo? RodeoOrigen { get; set; }

        [ObservableProperty]
        public partial Rodeo? RodeoDestino { get; set; }

        // El selector solo se muestra con un origen que tiene animales (R1)
        public bool MostrarSelector => RodeoOrigen is { CantidadAnimales: > 0 };

        public bool MostrarElegirOrigen => RodeoOrigen is null;

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorOrigen))]
        public partial string? ErrorOrigen { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorDestino))]
        public partial string? ErrorDestino { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorAnimales))]
        public partial string? ErrorAnimales { get; set; }

        public bool HayErrorOrigen => !string.IsNullOrEmpty(ErrorOrigen);
        public bool HayErrorDestino => !string.IsNullOrEmpty(ErrorDestino);
        public bool HayErrorAnimales => !string.IsNullOrEmpty(ErrorAnimales);

        // ===== Cambios que mueven el selector =====

        // Cambiar el origen limpia la selección y carga los animales del rodeo nuevo
        partial void OnRodeoOrigenChanged(Rodeo? oldValue, Rodeo? newValue)
        {
            if (_ajustandoPickers || oldValue?.Id == newValue?.Id)
                return;

            _ = ActualizarOrigenAsync(limpiarSeleccion: true);
        }

        // Destino sin el origen, aviso de R1 y animales del selector.
        // Se limpia la selección a mano: CambiarOrigenAsync con el MISMO origen la conservaría,
        // y con un origen sin animales el selector ni se llama
        private async Task ActualizarOrigenAsync(bool limpiarSeleccion)
        {
            if (limpiarSeleccion || !MostrarSelector)
                Selector.LimpiarSeleccionCommand.Execute(null);

            ErrorAnimales = null;
            ActualizarRodeosDestino();

            // R1: se avisa al elegirlo y no deja continuar
            ErrorOrigen = RodeoOrigen is { CantidadAnimales: <= 0 } ? MensajeOrigenSinAnimales : null;

            if (MostrarSelector)
                await Selector.CambiarOrigenAsync(OrigenAnimales.DeRodeo(RodeoOrigen!.Id));
        }

        // R3: el origen no aparece entre los destinos. Si el destino elegido pasa a ser
        // el origen, se borra; si no, se conserva (se vuelve a buscar por id en la lista nueva)
        private void ActualizarRodeosDestino()
        {
            var idDestino = RodeoDestino?.Id;
            var idOrigen = RodeoOrigen?.Id;

            _ajustandoPickers = true;
            try
            {
                var destinos = Rodeos.Where(r => r.Id != idOrigen).ToList();
                RodeosDestino = destinos;
                RodeoDestino = destinos.FirstOrDefault(r => r.Id == idDestino);
            }
            finally
            {
                _ajustandoPickers = false;
            }
        }

        // ===== Carga =====

        // Al aparecer. Conserva origen y destino (por id) si siguen en la lista
        [RelayCommand]
        private Task CargarAsync() => EjecutarAsync(async () =>
        {
            var resultado = await _rodeoService.ListarAsync();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                return;
            }

            var idOrigen = RodeoOrigen?.Id;
            var rodeos = resultado.Datos ?? [];

            _ajustandoPickers = true;
            try
            {
                Rodeos = rodeos;
                RodeoOrigen = rodeos.FirstOrDefault(r => r.Id == idOrigen);
            }
            finally
            {
                _ajustandoPickers = false;
            }

            // Si el origen ya no está (se eliminó), su selección no vale más.
            // Si sigue, se recargan sus animales conservando lo marcado
            await ActualizarOrigenAsync(limpiarSeleccion: RodeoOrigen?.Id != idOrigen);
        });

        // ===== Transferir =====

        [RelayCommand]
        private async Task TransferirAsync()
        {
            MensajeError = null;

            // Se validan los tres campos a la vez, para marcar cada uno con su error
            var origen = RodeoOrigen;
            var destino = RodeoDestino;

            ErrorOrigen = origen switch
            {
                null => "El rodeo origen es obligatorio.",
                { CantidadAnimales: <= 0 } => MensajeOrigenSinAnimales,
                _ => null
            };

            // E2: destino obligatorio (y distinto del origen, aunque el Picker ya no lo ofrece)
            ErrorDestino = destino switch
            {
                null => "El rodeo destino es obligatorio.",
                _ when destino.Id == origen?.Id => "El rodeo destino debe ser distinto al rodeo origen.",
                _ => null
            };

            // R2 / E3: solo tiene sentido con un origen que muestra animales
            ErrorAnimales = MostrarSelector && Selector.CantidadSeleccionados == 0
                ? "Debe seleccionar al menos un animal."
                : null;

            if (HayErrorOrigen || HayErrorDestino || HayErrorAnimales)
                return;

            var pedido = new TransferirAnimalesRequest
            {
                IdRodeoOrigen = origen!.Id,
                IdRodeoDestino = destino!.Id,
                AnimalesIds = Selector.Seleccionados.Select(a => a.Id).ToList()
            };

            var cantidad = pedido.AnimalesIds.Count;
            var textoAnimales = cantidad == 1 ? "1 animal" : $"{cantidad} animales";

            var confirmado = await Shell.Current.DisplayAlertAsync(
                "Transferir animales",
                $"¿Transferir {textoAnimales} de {origen.Nombre} a {destino.Nombre}?",
                "Transferir",
                "Cancelar");

            // E4: si cancela, no se llama a la API
            if (!confirmado)
                return;

            TransferenciaResponse? transferencia = null;
            var exito = false;
            var recargarSelector = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _rodeoService.TransferirAsync(pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    transferencia = resultado.Datos;
                    return;
                }

                // Se muestra el mensaje del back y el formulario conserva lo cargado.
                // R5: es atómica, no hay transferencias parciales que reflejar
                MensajeError = resultado.MensajeError;

                // Un animal que ya no está en el origen llega como 400. Los otros 400 (destino igual,
                // sin animales) ya los frena la validación local, así que se recarga ante cualquier 400:
                // el selector conserva marcados los que siguen en el origen
                recargarSelector = resultado.CodigoEstado == HttpStatusCode.BadRequest;
            });

            if (recargarSelector)
            {
                await Selector.RecargarAsync();
                return;
            }

            if (!exito)
                return;

            // S2, S3: lo que confirma el back (si no llegara la respuesta, lo que se envió)
            var transferidos = transferencia?.CantidadAnimalesTransferidos ?? cantidad;
            var nombreDestino = string.IsNullOrEmpty(transferencia?.NombreRodeoDestino)
                ? destino.Nombre
                : transferencia.NombreRodeoDestino;

            await Shell.Current.DisplayAlertAsync("Transferir animales",
                $"Transferencia realizada con éxito.\n\nAnimales transferidos: {transferidos}\nRodeo destino: {nombreDestino}",
                "Aceptar");

            // La lista se recarga al volver (RodeosPage.OnAppearing) con las cantidades nuevas
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");
    }
}
