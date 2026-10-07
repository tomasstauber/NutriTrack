using System.Globalization;
using System.Net;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriTrack.MAUI.Helpers;
using NutriTrack.MAUI.Models;
using NutriTrack.MAUI.Services;

namespace NutriTrack.MAUI.ViewModels
{
    // CU13 - Registrar peso de un animal
    // Se llega desde el panel o desde el perfil del animal, que pasa
    // "caravanaCuig" y "caravanaNroManejo" para precargar la búsqueda
    public partial class RegistrarPesoViewModel : BaseViewModel, IQueryAttributable
    {
        private const string MensajeAnimalNoEncontrado = "No se encontró un animal activo con esa caravana";

        private readonly IAnimalService _animalService;
        private readonly IRegistroPesoService _registroPesoService;

        // Id del animal encontrado: la ficha no lo trae, sale del listado
        private int? _idAnimal;

        public RegistrarPesoViewModel(IAnimalService animalService, IRegistroPesoService registroPesoService)
        {
            _animalService = animalService;
            _registroPesoService = registroPesoService;
        }

        // Paso 1: caravana

        [ObservableProperty]
        public partial string? Cuig { get; set; }

        [ObservableProperty]
        public partial string? NroManejo { get; set; }

        // Animal encontrado (null hasta buscar uno activo)
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayAnimal))]
        [NotifyPropertyChangedFor(nameof(Caravana))]
        [NotifyPropertyChangedFor(nameof(TextoUltimoPeso))]
        [NotifyCanExecuteChangedFor(nameof(GuardarCommand))]
        public partial FichaAnimal? Animal { get; set; }

        public bool HayAnimal => Animal is not null;

        public string Caravana => Animal is null
            ? string.Empty
            : CaravanaValidador.Formatear(Animal.CaravanaCuig, Animal.CaravanaNroManejo);

        // La fecha llega con Z y se muestra tal cual, sin pasar a hora local
        public string TextoUltimoPeso => Animal?.UltimoPeso is { } ultimo
            ? $"{FormatearPeso(ultimo.PesoKg)} kg ({ultimo.FechaPesaje:dd/MM/yyyy})"
            : "Sin pesajes";

        // Paso 2: pesaje

        [ObservableProperty]
        public partial string? Peso { get; set; }

        [ObservableProperty]
        public partial DateTime? FechaPesaje { get; set; } = DateTime.Today;

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        // R3: no se puede elegir una fecha posterior a hoy
        public DateTime Hoy => DateTime.Today;

        // Si cambia la caravana, el animal encontrado ya no corresponde
        partial void OnCuigChanged(string? value) => Animal = null;

        partial void OnNroManejoChanged(string? value) => Animal = null;

        // Caravana precargada desde el perfil del animal
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (!query.TryGetValue("caravanaCuig", out var cuig) ||
                !query.TryGetValue("caravanaNroManejo", out var nroManejo))
                return;

            Cuig = cuig?.ToString();
            NroManejo = nroManejo?.ToString();
            BuscarCommand.Execute(null);
        }

        [RelayCommand]
        private async Task BuscarAsync()
        {
            Animal = null;

            var cuig = Cuig?.Trim();
            var nroManejo = NroManejo?.Trim();

            // E1: formato inválido
            if (!CaravanaValidador.EsValida(cuig, nroManejo))
            {
                MensajeError = MensajeAnimalNoEncontrado;
                return;
            }

            await EjecutarAsync(async () =>
            {
                // Primero el listado: no distingue mayúsculas ("ar001" encuentra "AR001")
                // y trae el id, que la ficha no tiene. Solo devuelve animales activos
                var animal = await _animalService.BuscarActivoPorCaravanaAsync(cuig!, nroManejo!);

                if (!animal.Exito)
                {
                    MensajeError = animal.MensajeError;
                    return;
                }

                // E1: caravana inexistente o animal inactivo
                if (animal.Datos is null)
                {
                    MensajeError = MensajeAnimalNoEncontrado;
                    return;
                }

                // La ficha compara la caravana exacta: se pide tal como está guardada
                var ficha = await _animalService.ObtenerFichaAsync(
                    animal.Datos.CaravanaCuig, animal.Datos.CaravanaNroManejo);

                if (!ficha.Exito)
                {
                    // E1: el animal dejó de existir entre los dos pedidos (404)
                    MensajeError = ficha.CodigoEstado is HttpStatusCode.NotFound or HttpStatusCode.BadRequest
                        ? MensajeAnimalNoEncontrado
                        : ficha.MensajeError;
                    return;
                }

                // E1: animal inactivo
                if (ficha.Datos is null || ficha.Datos.Estado != "Activo")
                {
                    MensajeError = MensajeAnimalNoEncontrado;
                    return;
                }

                _idAnimal = animal.Datos.Id;
                Animal = ficha.Datos;
            });
        }

        [RelayCommand(CanExecute = nameof(HayAnimal))]
        private async Task GuardarAsync()
        {
            if (Animal is null || _idAnimal is null)
                return;

            // R2 / E3: peso mayor a 0 (acepta coma o punto decimal)
            if (!IntentarLeerPeso(Peso, out var pesoKg) || pesoKg <= 0)
            {
                MensajeError = "El peso debe ser mayor a 0";
                return;
            }

            // R3 / E4: fecha no posterior a hoy
            var fecha = (FechaPesaje ?? DateTime.Today).Date;
            if (fecha > DateTime.Today)
            {
                MensajeError = "La fecha del pesaje no puede ser posterior a la fecha actual";
                return;
            }

            var observaciones = Observaciones?.Trim();
            var registro = new RegistroPesoRequest
            {
                IdAnimal = _idAnimal.Value,
                FechaPesaje = DateOnly.FromDateTime(fecha),
                PesoKg = pesoKg,
                Observaciones = string.IsNullOrEmpty(observaciones) ? null : observaciones
            };

            var caravana = Caravana;
            var registrado = false;

            await EjecutarAsync(async () =>
            {
                var resultado = await _registroPesoService.RegistrarAsync(registro);

                if (resultado.Exito)
                    registrado = true;
                else
                    MensajeError = resultado.MensajeError;
            });

            if (!registrado)
                return;

            // S2 a S4: se muestra lo que se envió
            await Shell.Current.DisplayAlertAsync(
                "Registrar peso",
                "Peso registrado correctamente.\n\n" +
                $"Caravana: {caravana}\n" +
                $"Fecha: {fecha:dd/MM/yyyy}\n" +
                $"Peso: {FormatearPeso(pesoKg)} kg",
                "Aceptar");

            PrepararOtroPesaje();
        }

        // Deja la pantalla lista para otro pesaje. El CUIG se conserva porque
        // suele ser el mismo para todos los animales del establecimiento
        private void PrepararOtroPesaje()
        {
            NroManejo = null;
            Animal = null;
            _idAnimal = null;
            Peso = null;
            FechaPesaje = DateTime.Today;
            Observaciones = null;
            MensajeError = null;
        }

        private static bool IntentarLeerPeso(string? texto, out decimal pesoKg)
        {
            var normalizado = texto?.Trim().Replace(',', '.');
            return decimal.TryParse(normalizado, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out pesoKg);
        }

        private static string FormatearPeso(decimal pesoKg) => pesoKg.ToString("0.##");
    }
}
