using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU12 - Registrar evento sanitario a múltiples animales
    // (#108: selección y datos; #109: medicamentos y guardado)
    public partial class RegistrarEventoSanitarioViewModel : BaseViewModel
    {
        private const string MensajeRodeoSinAnimales =
            "No se encontró el rodeo seleccionado o no tiene animales activos.";

        private const string MensajeExito = "Evento sanitario registrado con éxito.";

        private readonly IRodeoService _rodeoService;
        private readonly IMedicamentoService _medicamentoService;
        private readonly IEventoSanitarioService _eventoSanitarioService;

        // true mientras se reemplaza la lista de rodeos: el Picker puede pasar por null
        // y eso no tiene que tocar la selección de animales
        private bool _recargandoRodeos;

        public RegistrarEventoSanitarioViewModel(
            IRodeoService rodeoService,
            IMedicamentoService medicamentoService,
            IEventoSanitarioService eventoSanitarioService,
            SelectorAnimalesViewModel selector)
        {
            _rodeoService = rodeoService;
            _medicamentoService = medicamentoService;
            _eventoSanitarioService = eventoSanitarioService;
            Selector = selector;
        }

        // Selector múltiple de animales (#101): la vista lo recibe como BindingContext
        public SelectorAnimalesViewModel Selector { get; }

        // ===== Modo de selección =====

        public IReadOnlyList<OpcionModoSeleccion> OpcionesModo { get; } =
        [
            new() { Texto = "Rodeo completo", Valor = ModosSeleccionEvento.RodeoCompleto },
            new() { Texto = "Selección manual", Valor = ModosSeleccionEvento.SeleccionManual },
            new() { Texto = "Selección libre", Valor = ModosSeleccionEvento.SeleccionLibre }
        ];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EsRodeoCompleto))]
        [NotifyPropertyChangedFor(nameof(EsSeleccionManual))]
        [NotifyPropertyChangedFor(nameof(EsSeleccionLibre))]
        [NotifyPropertyChangedFor(nameof(MostrarRodeo))]
        [NotifyPropertyChangedFor(nameof(MostrarAlcance))]
        [NotifyPropertyChangedFor(nameof(MostrarSelector))]
        [NotifyPropertyChangedFor(nameof(MostrarElegirRodeo))]
        public partial OpcionModoSeleccion? ModoSeleccionado { get; set; }

        public bool EsRodeoCompleto => ModoSeleccionado?.Valor == ModosSeleccionEvento.RodeoCompleto;
        public bool EsSeleccionManual => ModoSeleccionado?.Valor == ModosSeleccionEvento.SeleccionManual;
        public bool EsSeleccionLibre => ModoSeleccionado?.Valor == ModosSeleccionEvento.SeleccionLibre;

        // ===== Rodeo =====

        // Solo rodeos activos (GET api/Rodeo)
        [ObservableProperty]
        public partial IReadOnlyList<Rodeo> Rodeos { get; set; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarAlcance))]
        [NotifyPropertyChangedFor(nameof(TextoAlcance))]
        [NotifyPropertyChangedFor(nameof(MostrarSelector))]
        [NotifyPropertyChangedFor(nameof(MostrarElegirRodeo))]
        public partial Rodeo? RodeoSeleccionado { get; set; }

        // Rodeo completo y Selección manual eligen rodeo; Selección libre no
        public bool MostrarRodeo => EsRodeoCompleto || EsSeleccionManual;

        // Rodeo completo: a cuántos animales alcanza. cantidadAnimales cuenta solo
        // los activos, que son los mismos que alcanza el modo en el back
        public bool MostrarAlcance => EsRodeoCompleto && RodeoSeleccionado is not null;

        public string TextoAlcance => RodeoSeleccionado?.CantidadAnimales switch
        {
            null => string.Empty,
            1 => "Alcanza 1 animal activo",
            var cantidad => $"Alcanza {cantidad} animales activos"
        };

        // ===== Animales (selector) =====

        // Manual: los animales del rodeo elegido. Libre: todos los activos
        public bool MostrarSelector => EsSeleccionLibre || (EsSeleccionManual && RodeoSeleccionado is not null);

        public bool MostrarElegirRodeo => EsSeleccionManual && RodeoSeleccionado is null;

        // ===== Datos del evento =====

        public IReadOnlyList<OpcionTipoEvento> OpcionesTipo { get; } =
        [
            new() { Texto = "Vacunación", Valor = "Vacunacion" },
            new() { Texto = "Desparasitación", Valor = "Desparasitacion" },
            new() { Texto = "Tratamiento", Valor = "Tratamiento" },
            new() { Texto = "Refuerzo", Valor = "Refuerzo" },
            new() { Texto = "Control", Valor = "Control" },
            new() { Texto = "Otro", Valor = "Otro" }
        ];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EsTipoOtro))]
        public partial OpcionTipoEvento? TipoSeleccionado { get; set; }

        // Con "Otro" se sugiere detallar el evento en observaciones
        public bool EsTipoOtro => TipoSeleccionado?.Valor == "Otro";

        [ObservableProperty]
        public partial DateTime? FechaEvento { get; set; } = DateTime.Today;

        // Vigencia y próxima aplicación son opcionales: un DatePicker siempre tiene fecha,
        // así que cada una tiene un CheckBox. Sin tildar, viajan en null
        [ObservableProperty]
        public partial bool CargarVigencia { get; set; }

        [ObservableProperty]
        public partial DateTime? VigenciaHasta { get; set; }

        [ObservableProperty]
        public partial bool CargarProximaAplicacion { get; set; }

        [ObservableProperty]
        public partial DateTime? FechaProximaAplicacion { get; set; }

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        // R7: no se puede elegir una fecha del evento posterior a hoy
        public DateTime Hoy => DateTime.Today;

        // ===== Medicamentos =====

        // Catálogo activo (GET api/Medicamento sin incluirInactivos)
        private IReadOnlyList<Medicamento> _catalogoMedicamentos = [];

        // Cero, uno o varios: sin medicamentos el evento se guarda igual
        public ObservableCollection<DetalleMedicamentoEditable> Medicamentos { get; } = [];

        // ===== Errores de cada campo (MensajeError queda para los de la API) =====

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorModo))]
        public partial string? ErrorModo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorRodeo))]
        public partial string? ErrorRodeo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorAnimales))]
        public partial string? ErrorAnimales { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorTipo))]
        public partial string? ErrorTipo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorFechaEvento))]
        public partial string? ErrorFechaEvento { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorVigencia))]
        public partial string? ErrorVigencia { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorProximaAplicacion))]
        public partial string? ErrorProximaAplicacion { get; set; }

        public bool HayErrorModo => !string.IsNullOrEmpty(ErrorModo);
        public bool HayErrorRodeo => !string.IsNullOrEmpty(ErrorRodeo);
        public bool HayErrorAnimales => !string.IsNullOrEmpty(ErrorAnimales);
        public bool HayErrorTipo => !string.IsNullOrEmpty(ErrorTipo);
        public bool HayErrorFechaEvento => !string.IsNullOrEmpty(ErrorFechaEvento);
        public bool HayErrorVigencia => !string.IsNullOrEmpty(ErrorVigencia);
        public bool HayErrorProximaAplicacion => !string.IsNullOrEmpty(ErrorProximaAplicacion);

        // ===== Cambios que mueven el selector =====

        // Cambiar de modo limpia la selección y configura el origen del selector
        partial void OnModoSeleccionadoChanged(OpcionModoSeleccion? value) => _ = ActualizarSelectorAsync();

        // Cambiar de rodeo limpia la selección (en manual, el origen pasa a ser el rodeo nuevo)
        partial void OnRodeoSeleccionadoChanged(Rodeo? oldValue, Rodeo? newValue)
        {
            if (_recargandoRodeos || oldValue?.Id == newValue?.Id)
                return;

            _ = ActualizarSelectorAsync();
        }

        // Al tildar una fecha opcional, arranca en la fecha del evento (cumple R11 / R12)
        partial void OnCargarVigenciaChanged(bool value)
        {
            if (value)
                VigenciaHasta = FechaEvento ?? DateTime.Today;
        }

        partial void OnCargarProximaAplicacionChanged(bool value)
        {
            if (value)
                FechaProximaAplicacion = FechaEvento ?? DateTime.Today;
        }

        // Siempre se limpia primero: CambiarOrigenAsync con el MISMO origen conserva
        // la selección (por ejemplo, manual → rodeo completo → manual con el mismo rodeo)
        private async Task ActualizarSelectorAsync()
        {
            Selector.LimpiarSeleccionCommand.Execute(null);

            OrigenAnimales? origen = ModoSeleccionado?.Valor switch
            {
                ModosSeleccionEvento.SeleccionManual when RodeoSeleccionado is not null =>
                    OrigenAnimales.DeRodeo(RodeoSeleccionado.Id),
                ModosSeleccionEvento.SeleccionLibre => OrigenAnimales.Todos,
                _ => null
            };

            // Rodeo completo, o manual sin rodeo: el selector no se muestra
            if (origen is not null)
                await Selector.CambiarOrigenAsync(origen);
        }

        // ===== Carga =====

        // Al aparecer: rodeos y catálogo de medicamentos
        [RelayCommand]
        private Task CargarCatalogosAsync() => EjecutarAsync(async () =>
        {
            await CargarRodeosAsync();
            await CargarMedicamentosAsync();
        });

        // Conserva el rodeo elegido (por id) si sigue en la lista
        private async Task CargarRodeosAsync()
        {
            var resultado = await _rodeoService.ListarAsync();

            if (!resultado.Exito)
            {
                MensajeError = resultado.MensajeError;
                return;
            }

            var idAnterior = RodeoSeleccionado?.Id;
            var rodeos = resultado.Datos ?? [];

            _recargandoRodeos = true;
            try
            {
                Rodeos = rodeos;
                RodeoSeleccionado = rodeos.FirstOrDefault(r => r.Id == idAnterior);
            }
            finally
            {
                _recargandoRodeos = false;
            }

            // El rodeo elegido ya no está (se eliminó): la selección de ese rodeo no vale más
            if (idAnterior is not null && RodeoSeleccionado is null)
                await ActualizarSelectorAsync();
        }

        // Solo los activos. Las filas ya cargadas conservan su medicamento si sigue activo
        private async Task CargarMedicamentosAsync()
        {
            var resultado = await _medicamentoService.ListarAsync();

            if (!resultado.Exito)
            {
                // Si ya falló la carga de rodeos, queda ese mensaje
                MensajeError ??= resultado.MensajeError;
                return;
            }

            _catalogoMedicamentos = (resultado.Datos ?? []).Where(m => m.Activo).ToList();

            foreach (var detalle in Medicamentos)
                detalle.ActualizarMedicamentos(_catalogoMedicamentos);
        }

        [RelayCommand]
        private void AgregarMedicamento() =>
            Medicamentos.Add(new DetalleMedicamentoEditable(_catalogoMedicamentos));

        [RelayCommand]
        private void QuitarMedicamento(DetalleMedicamentoEditable detalle) =>
            Medicamentos.Remove(detalle);

        // ===== Guardar =====

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            var modo = ModoSeleccionado?.Valor;

            ErrorModo = modo is null ? "El modo de selección es obligatorio." : null;

            // Rodeo completo y Selección manual: rodeo obligatorio y con animales activos (R1 / E2)
            var usaRodeo = modo is ModosSeleccionEvento.RodeoCompleto or ModosSeleccionEvento.SeleccionManual;
            ErrorRodeo = !usaRodeo ? null
                : RodeoSeleccionado is null ? "El rodeo es obligatorio."
                : RodeoSeleccionado.CantidadAnimales == 0 ? MensajeRodeoSinAnimales
                : null;

            // R2: manual y libre, al menos un animal. En manual sin rodeo ya marca el rodeo
            var usaAnimales = modo == ModosSeleccionEvento.SeleccionLibre ||
                              (modo == ModosSeleccionEvento.SeleccionManual && RodeoSeleccionado is not null);
            ErrorAnimales = usaAnimales && Selector.CantidadSeleccionados == 0
                ? "Debe seleccionar al menos un animal."
                : null;

            ErrorTipo = TipoSeleccionado is null ? "El tipo de evento es obligatorio." : null;

            // R7: fecha del evento obligatoria y no posterior a hoy
            var fechaEvento = FechaEvento?.Date;
            ErrorFechaEvento = fechaEvento switch
            {
                null => "La fecha del evento es obligatoria.",
                _ when fechaEvento > DateTime.Today => "La fecha del evento no puede ser posterior a hoy.",
                _ => null
            };

            // R11 / R12: si se cargan, iguales o posteriores a la fecha del evento
            ErrorVigencia = ValidarFechaOpcional(CargarVigencia, VigenciaHasta, fechaEvento,
                "La vigencia debe ser igual o posterior a la fecha del evento.");

            ErrorProximaAplicacion = ValidarFechaOpcional(CargarProximaAplicacion, FechaProximaAplicacion, fechaEvento,
                "La próxima aplicación debe ser igual o posterior a la fecha del evento.");

            // R9 / R10: cada fila marca sus propios errores. Se validan todas (sin cortar en la primera)
            var medicamentosValidos = true;
            foreach (var detalle in Medicamentos)
                medicamentosValidos &= detalle.Validar();

            if (HayErrorModo || HayErrorRodeo || HayErrorAnimales || HayErrorTipo ||
                HayErrorFechaEvento || HayErrorVigencia || HayErrorProximaAplicacion || !medicamentosValidos)
                return;

            var pedido = ArmarPedido(modo!, fechaEvento!.Value);

            EventoSanitarioRegistrado? registrado = null;
            var exito = false;

            // EjecutarAsync deja EstaOcupado en true: Guardar queda deshabilitado
            // (el registro no es atómico y un doble envío duplica eventos)
            await EjecutarAsync(async () =>
            {
                var resultado = await _eventoSanitarioService.RegistrarMultipleAsync(pedido);

                if (resultado.Exito)
                {
                    exito = true;
                    registrado = resultado.Datos;
                    return;
                }

                // E2, E3 y E4: el back dice qué falla (rodeo sin animales, caravanas inactivas o
                // ajenas al rodeo, medicamento desactivado). Se conserva todo lo cargado
                MensajeError = resultado.MensajeError;
            });

            if (!exito)
                return;

            // S2 a S4: lo que devuelve el back (si no llegara la respuesta, lo que se envió)
            var tipo = registrado?.TipoEvento ?? pedido.TipoEvento;
            var textoTipo = OpcionesTipo.FirstOrDefault(o => o.Valor == tipo)?.Texto ?? tipo;
            var fecha = registrado?.FechaEvento ?? fechaEvento.Value;
            var cantidad = registrado is null
                ? string.Empty
                : $"Animales alcanzados: {registrado.CantidadAnimalesAlcanzados}\n";

            await Shell.Current.DisplayAlertAsync("Registrar evento sanitario",
                $"{MensajeExito}\n\n{cantidad}Tipo: {textoTipo}\nFecha: {fecha:dd/MM/yyyy}", "Aceptar");

            LimpiarFormulario();
        }

        // Todo lo que necesita el POST
        private RegistrarEventoSanitarioRequest ArmarPedido(string modo, DateTime fechaEvento)
        {
            var observaciones = Observaciones?.Trim();

            return new RegistrarEventoSanitarioRequest
            {
                // Selección libre: idRodeo viaja en null
                IdRodeo = modo == ModosSeleccionEvento.SeleccionLibre ? null : RodeoSeleccionado!.Id,
                ModoSeleccion = modo,

                // Rodeo completo no usa caravanas. En manual y libre van tal como las devolvió
                // el selector (el back distingue mayúsculas). El selector guarda por id: sin repetidas (R5)
                Caravanas = modo == ModosSeleccionEvento.RodeoCompleto
                    ? null
                    : Selector.Seleccionados
                        .Select(a => new CaravanaRequest
                        {
                            CaravanaCuig = a.CaravanaCuig,
                            CaravanaNroManejo = a.CaravanaNroManejo
                        })
                        .ToList(),

                TipoEvento = TipoSeleccionado!.Valor,
                FechaEvento = DateOnly.FromDateTime(fechaEvento),
                VigenciaHasta = CargarVigencia && VigenciaHasta is { } vigencia
                    ? DateOnly.FromDateTime(vigencia)
                    : null,
                FechaProximaAplicacion = CargarProximaAplicacion && FechaProximaAplicacion is { } proxima
                    ? DateOnly.FromDateTime(proxima)
                    : null,
                Observaciones = string.IsNullOrEmpty(observaciones) ? null : observaciones,

                // Sin medicamentos viaja una lista vacía
                DetallesMedicamento = Medicamentos.Select(d => d.ArmarPedido()).ToList()
            };
        }

        // Después de registrar: el formulario vuelve a como estaba al entrar
        private void LimpiarFormulario()
        {
            // Quitar el modo y el rodeo también limpia la selección del selector
            ModoSeleccionado = null;
            RodeoSeleccionado = null;
            Selector.LimpiarSeleccionCommand.Execute(null);

            TipoSeleccionado = null;
            FechaEvento = DateTime.Today;
            CargarVigencia = false;
            VigenciaHasta = null;
            CargarProximaAplicacion = false;
            FechaProximaAplicacion = null;
            Observaciones = null;
            Medicamentos.Clear();

            ErrorModo = null;
            ErrorRodeo = null;
            ErrorAnimales = null;
            ErrorTipo = null;
            ErrorFechaEvento = null;
            ErrorVigencia = null;
            ErrorProximaAplicacion = null;
            MensajeError = null;
        }

        // Sin tildar no se valida (viaja en null). Tildada: obligatoria y no anterior a la fecha del evento
        private static string? ValidarFechaOpcional(bool cargar, DateTime? fecha, DateTime? fechaEvento, string mensajeAnterior)
        {
            if (!cargar)
                return null;

            if (fecha is null)
                return "Elegí la fecha o destildá la opción.";

            return fechaEvento is not null && fecha.Value.Date < fechaEvento
                ? mensajeAnterior
                : null;
        }
    }
}
