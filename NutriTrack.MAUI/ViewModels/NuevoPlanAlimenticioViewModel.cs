using System.Collections.ObjectModel;
using System.Globalization;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU9 - Crear plan alimenticio y CU11 - Editar plan alimenticio (mismo formulario),
    // con la grilla de componentes
    public partial class NuevoPlanAlimenticioViewModel : BaseViewModel, IQueryAttributable
    {
        // Ruta de Shell del formulario (se registra en AppShell.xaml.cs)
        public const string Ruta = "nuevo-plan-alimenticio";

        // E4: texto del issue
        private const string MensajeSumaSupera100 =
            "La suma de los porcentajes de inclusión supera el 100%; ajuste los valores antes de continuar.";

        private readonly IPlanAlimenticioService _planAlimenticioService;
        private readonly IIngredienteService _ingredienteService;

        // CU27 R5 en el plan: texto del error de la grilla
        private const string MensajeIngredientesInactivos =
            "Hay componentes con ingredientes inactivos. Quitalos o reemplazalos por otro ingrediente antes de guardar.";

        // El catálogo (y en la edición, el plan) se pide una sola vez: si se recarga
        // al volver a la página, el Picker pierde lo que se estaba eligiendo
        private bool _datosCargados;

        // CU11: id del plan que se edita (null en el alta)
        private int? _idEdicion;

        // R8: rodeos con el plan asignado y vigente, según el GET por id
        private int _asignacionesVigentes;

        public bool EsEdicion => _idEdicion is not null;

        // Título de la página y de los avisos según el modo
        [ObservableProperty]
        public partial string Titulo { get; set; } = "Nuevo plan alimenticio";

        public NuevoPlanAlimenticioViewModel(IPlanAlimenticioService planAlimenticioService,
            IIngredienteService ingredienteService)
        {
            _planAlimenticioService = planAlimenticioService;
            _ingredienteService = ingredienteService;

            // R6: la suma se recalcula en vivo cada vez que se agrega o quita una fila
            Componentes.CollectionChanged += (_, _) => ActualizarSuma();
        }

        // CU11: la lista pasa solo el id; el plan con sus componentes se pide al cargar
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("idPlan", out var valor) || valor is not int idPlan)
                return;

            _idEdicion = idPlan;
            Titulo = "Editar plan alimenticio";
        }

        // ===== Datos del plan (D1 a D9) =====

        [ObservableProperty]
        public partial string? Nombre { get; set; }

        [ObservableProperty]
        public partial string? Categoria { get; set; }

        // Texto de los Entry: se convierten a número al guardar (acepta coma o punto)

        [ObservableProperty]
        public partial string? PesoVivoInicialPromedio { get; set; }

        [ObservableProperty]
        public partial string? PesoObjetivo { get; set; }

        [ObservableProperty]
        public partial string? GananciaPesoEsperada { get; set; }

        [ObservableProperty]
        public partial string? TipoAlimentacion { get; set; }

        [ObservableProperty]
        public partial string? TiempoAlimentacion { get; set; }

        [ObservableProperty]
        public partial string? KgMsDiariaPorAnimal { get; set; }

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        // Error de cada campo (null si el campo está bien).
        // MensajeError (BaseViewModel) queda para los errores de la API

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorNombre))]
        public partial string? ErrorNombre { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorPesoVivoInicialPromedio))]
        public partial string? ErrorPesoVivoInicialPromedio { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorPesoObjetivo))]
        public partial string? ErrorPesoObjetivo { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorGananciaPesoEsperada))]
        public partial string? ErrorGananciaPesoEsperada { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorTipoAlimentacion))]
        public partial string? ErrorTipoAlimentacion { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorKgMsDiariaPorAnimal))]
        public partial string? ErrorKgMsDiariaPorAnimal { get; set; }

        public bool HayErrorNombre => !string.IsNullOrEmpty(ErrorNombre);
        public bool HayErrorPesoVivoInicialPromedio => !string.IsNullOrEmpty(ErrorPesoVivoInicialPromedio);
        public bool HayErrorPesoObjetivo => !string.IsNullOrEmpty(ErrorPesoObjetivo);
        public bool HayErrorGananciaPesoEsperada => !string.IsNullOrEmpty(ErrorGananciaPesoEsperada);
        public bool HayErrorTipoAlimentacion => !string.IsNullOrEmpty(ErrorTipoAlimentacion);
        public bool HayErrorKgMsDiariaPorAnimal => !string.IsNullOrEmpty(ErrorKgMsDiariaPorAnimal);

        // ===== Grilla de componentes =====

        // Catálogo activo para el Picker (GET api/Ingrediente)
        [ObservableProperty]
        public partial IReadOnlyList<Ingrediente> Ingredientes { get; set; } = [];

        public ObservableCollection<ComponentePlanFila> Componentes { get; } = [];

        // Campos de "Agregar componente"

        [ObservableProperty]
        public partial Ingrediente? IngredienteSeleccionado { get; set; }

        [ObservableProperty]
        public partial string? PorcentajeInclusion { get; set; }

        [ObservableProperty]
        public partial string? ObservacionesComponente { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorIngrediente))]
        public partial string? ErrorIngrediente { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorPorcentajeInclusion))]
        public partial string? ErrorPorcentajeInclusion { get; set; }

        // R4: error de la grilla completa (sin componentes)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorComponentes))]
        public partial string? ErrorComponentes { get; set; }

        public bool HayErrorIngrediente => !string.IsNullOrEmpty(ErrorIngrediente);
        public bool HayErrorPorcentajeInclusion => !string.IsNullOrEmpty(ErrorPorcentajeInclusion);
        public bool HayErrorComponentes => !string.IsNullOrEmpty(ErrorComponentes);

        // R6: suma de porcentajes en vivo. Con decimal, no con double
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TextoSuma), nameof(SumaSupera100), nameof(MensajeSuma))]
        public partial decimal SumaPorcentajes { get; set; }

        public string TextoSuma => $"Suma de porcentajes: {SumaPorcentajes:0.##}%";

        public bool SumaSupera100 => SumaPorcentajes > 100;

        // E4: se muestra mientras la suma supere 100 (puede ser menor a 100)
        public string? MensajeSuma => SumaSupera100 ? MensajeSumaSupera100 : null;

        [RelayCommand]
        private async Task CargarAsync()
        {
            if (_datosCargados)
                return;

            var noExiste = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _ingredienteService.ListarAsync();

                if (!resultado.Exito)
                {
                    MensajeError = resultado.MensajeError;
                    return;
                }

                Ingredientes = resultado.Datos ?? [];

                // CU11: el catálogo va primero porque marca los componentes inactivos
                if (EsEdicion)
                {
                    var plan = await _planAlimenticioService.ObtenerAsync(_idEdicion!.Value);

                    if (!plan.Exito || plan.Datos is null)
                    {
                        noExiste = plan.CodigoEstado == HttpStatusCode.NotFound;
                        MensajeError = plan.MensajeError;
                        return;
                    }

                    Precargar(plan.Datos);
                }

                _datosCargados = true;
            });

            if (!noExiste)
                return;

            await Shell.Current.DisplayAlertAsync(Titulo, "No existe un plan con ese Id.", "Aceptar");

            // La lista se recarga al volver (PlanesAlimenticiosPage.OnAppearing)
            await Shell.Current.GoToAsync("..");
        }

        // CU11: formulario del alta precargado con el GET por id
        private void Precargar(PlanAlimenticioCompleto plan)
        {
            _asignacionesVigentes = plan.AsignacionesVigentes;

            Nombre = plan.NombrePlan;
            Categoria = plan.Categoria;
            PesoVivoInicialPromedio = plan.PesoVivoInicialPromedio?.ToString(CultureInfo.CurrentCulture);
            PesoObjetivo = plan.PesoObjetivo?.ToString(CultureInfo.CurrentCulture);
            GananciaPesoEsperada = plan.GananciaPesoEsperada?.ToString(CultureInfo.CurrentCulture);
            TipoAlimentacion = plan.TipoAlimentacion;
            TiempoAlimentacion = plan.TiempoAlimentacion;
            KgMsDiariaPorAnimal = plan.KgMsDiariaPorAnimal.ToString(CultureInfo.CurrentCulture);
            Observaciones = plan.Observaciones;

            // CU27 R5: un ingrediente que no está en el catálogo activo se marca como inactivo
            Componentes.Clear();
            foreach (var detalle in plan.Detalles)
            {
                var inactivo = Ingredientes.All(i => i.Id != detalle.IdIngrediente);
                Componentes.Add(new ComponentePlanFila(detalle, inactivo));
            }

            ErrorComponentes = Componentes.Any(c => c.IngredienteInactivo) ? MensajeIngredientesInactivos : null;
        }

        [RelayCommand]
        private void AgregarComponente()
        {
            ErrorComponentes = null;

            ErrorIngrediente = IngredienteSeleccionado is null ? "Elegí un ingrediente." : null;

            // R5 / E3: validación local, al agregar
            if (IngredienteSeleccionado is not null &&
                Componentes.Any(c => c.IdIngrediente == IngredienteSeleccionado.Id))
                ErrorIngrediente = "El ingrediente ya fue agregado al plan";

            // R4: porcentaje obligatorio, entre 0 y 100
            decimal porcentaje = 0;
            var textoPorcentaje = PorcentajeInclusion?.Trim();
            ErrorPorcentajeInclusion =
                string.IsNullOrEmpty(textoPorcentaje) ? "El porcentaje de inclusión es obligatorio."
                : !TryParseNumero(textoPorcentaje, out porcentaje) ? "El porcentaje de inclusión debe ser un número."
                : porcentaje < 0 || porcentaje > 100 ? "El porcentaje de inclusión debe estar entre 0 y 100."
                : null;

            if (HayErrorIngrediente || HayErrorPorcentajeInclusion)
                return;

            Componentes.Add(new ComponentePlanFila(IngredienteSeleccionado!, porcentaje,
                TextoOpcional(ObservacionesComponente)));

            // Se limpia para cargar el siguiente
            IngredienteSeleccionado = null;
            PorcentajeInclusion = null;
            ObservacionesComponente = null;
        }

        [RelayCommand]
        private void QuitarComponente(ComponentePlanFila fila)
        {
            Componentes.Remove(fila);

            // CU27 R5: al quitar el último componente inactivo se va el aviso
            if (ErrorComponentes == MensajeIngredientesInactivos && !Componentes.Any(c => c.IngredienteInactivo))
                ErrorComponentes = null;
        }

        [RelayCommand]
        private async Task GuardarAsync()
        {
            MensajeError = null;

            var nombre = Nombre?.Trim();
            var tipoAlimentacion = TipoAlimentacion?.Trim();

            // Se validan todos los campos a la vez, para marcar cada uno con su error (E2)

            // R1: nombre obligatorio
            ErrorNombre = string.IsNullOrEmpty(nombre) ? "El nombre del plan es obligatorio." : null;

            // Opcionales: si se cargan, tienen que ser números
            ErrorPesoVivoInicialPromedio = ValidarOpcional(PesoVivoInicialPromedio, "El peso vivo inicial promedio", out var pesoInicial);
            ErrorPesoObjetivo = ValidarOpcional(PesoObjetivo, "El peso objetivo", out var pesoObjetivo);
            ErrorGananciaPesoEsperada = ValidarOpcional(GananciaPesoEsperada, "La ganancia de peso esperada", out var ganancia);

            // R2: tipo de alimentación obligatorio
            ErrorTipoAlimentacion = string.IsNullOrEmpty(tipoAlimentacion) ? "El tipo de alimentación es obligatorio." : null;

            // R3: kg de MS obligatorio y mayor a 0
            decimal kgMs = 0;
            var textoKgMs = KgMsDiariaPorAnimal?.Trim();
            ErrorKgMsDiariaPorAnimal =
                string.IsNullOrEmpty(textoKgMs) ? "Los kg de MS diaria por animal son obligatorios."
                : !TryParseNumero(textoKgMs, out kgMs) ? "Los kg de MS diaria por animal deben ser un número."
                : kgMs <= 0 ? "Los kg de MS diaria por animal deben ser mayores a 0."
                : null;

            // R4: al menos un componente. CU27 R5: ninguno con el ingrediente inactivo
            ErrorComponentes =
                Componentes.Count == 0 ? "Agregá al menos un componente al plan."
                : Componentes.Any(c => c.IngredienteInactivo) ? MensajeIngredientesInactivos
                : null;

            // R6 / E4: con la suma por encima de 100 no se guarda (el mensaje ya está a la vista)
            if (HayErrorNombre || HayErrorPesoVivoInicialPromedio || HayErrorPesoObjetivo ||
                HayErrorGananciaPesoEsperada || HayErrorTipoAlimentacion || HayErrorKgMsDiariaPorAnimal ||
                HayErrorComponentes || SumaSupera100)
                return;

            var pedido = new PlanAlimenticioRequest
            {
                NombrePlan = nombre!,
                Categoria = TextoOpcional(Categoria),
                PesoVivoInicialPromedio = pesoInicial,
                PesoObjetivo = pesoObjetivo,
                GananciaPesoEsperada = ganancia,
                TipoAlimentacion = tipoAlimentacion!,
                TiempoAlimentacion = TextoOpcional(TiempoAlimentacion),
                KgMsDiariaPorAnimal = kgMs,
                Observaciones = TextoOpcional(Observaciones),
                Detalle = Componentes.Select(c => c.ArmarPedido()).ToList()
            };

            if (EsEdicion)
            {
                await ActualizarAsync(_idEdicion!.Value, pedido);
                return;
            }

            var creado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _planAlimenticioService.CrearAsync(pedido);

                if (resultado.Exito)
                {
                    creado = true;
                    return;
                }

                // E1: nombre duplicado (u otro error del back). Se muestra el mensaje
                // y el formulario conserva lo cargado, incluida la grilla
                MensajeError = resultado.MensajeError;
            });

            if (!creado)
                return;

            await Shell.Current.DisplayAlertAsync(Titulo, "Plan alimenticio creado con éxito.", "Aceptar");

            // La lista se recarga al volver y muestra el plan nuevo
            await Shell.Current.GoToAsync("..");
        }

        // CU11 - Editar plan alimenticio
        private async Task ActualizarAsync(int id, PlanAlimenticioRequest pedido)
        {
            // R8: con asignaciones vigentes se advierte antes de guardar
            if (_asignacionesVigentes > 0)
            {
                var confirmado = await Shell.Current.DisplayAlertAsync(
                    Titulo,
                    $"Este plan tiene {_asignacionesVigentes} asignación(es) vigente(s). " +
                    "Los cambios afectan a esos rodeos. ¿Querés guardar igual?",
                    "Guardar",
                    "Cancelar");

                if (!confirmado)
                    return;
            }

            var actualizado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _planAlimenticioService.ActualizarAsync(id, pedido);

                if (resultado.Exito)
                {
                    actualizado = true;
                    return;
                }

                // E1: nombre duplicado (excluye al propio plan) u otro error: mensaje del back.
                // El formulario conserva lo cargado, incluida la grilla
                MensajeError = resultado.MensajeError;
            });

            if (!actualizado)
                return;

            await Shell.Current.DisplayAlertAsync(Titulo, "Plan alimenticio actualizado con éxito.", "Aceptar");

            // La lista se recarga al volver y muestra los datos nuevos
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private Task Cancelar() => Shell.Current.GoToAsync("..");

        private void ActualizarSuma()
        {
            SumaPorcentajes = Componentes.Sum(c => c.PorcentajeInclusionMs);
        }

        // Vacío: sin error y viaja en null (no en 0)
        private static string? ValidarOpcional(string? texto, string campo, out decimal? valor)
        {
            valor = null;

            var recortado = texto?.Trim();
            if (string.IsNullOrEmpty(recortado))
                return null;

            if (!TryParseNumero(recortado, out var numero))
                return $"{campo} debe ser un número.";

            valor = numero;
            return null;
        }

        // Coma o punto como separador decimal, sin separador de miles.
        // AllowLeadingSign: un negativo se lee como número y recibe el mensaje de rango
        private static bool TryParseNumero(string texto, out decimal numero) =>
            decimal.TryParse(texto.Replace(',', '.'), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out numero);

        private static string? TextoOpcional(string? texto)
        {
            var recortado = texto?.Trim();
            return string.IsNullOrEmpty(recortado) ? null : recortado;
        }
    }
}
