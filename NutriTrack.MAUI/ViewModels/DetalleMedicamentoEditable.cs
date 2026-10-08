using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using NutriTrack.MAUI.Models;

namespace NutriTrack.MAUI.ViewModels
{
    // Fila de la sección de medicamentos del evento sanitario (#109):
    // lo que se carga de un medicamento aplicado y los errores de cada campo
    public partial class DetalleMedicamentoEditable : ObservableObject
    {
        public DetalleMedicamentoEditable(IReadOnlyList<Medicamento> medicamentos)
        {
            Medicamentos = medicamentos;
        }

        // Texto exacto del enum UnidadDosis del back (ojo con "UI", en mayúscula)
        public static IReadOnlyList<string> Unidades { get; } = ["ml", "l", "mg", "g", "UI", "cm3"];

        // Catálogo activo. Se reemplaza al recargar, por eso es observable
        [ObservableProperty]
        public partial IReadOnlyList<Medicamento> Medicamentos { get; set; }

        [ObservableProperty]
        public partial Medicamento? MedicamentoSeleccionado { get; set; }

        // Texto del Entry: se convierte a número al validar (acepta coma o punto)
        [ObservableProperty]
        public partial string? Dosis { get; set; }

        [ObservableProperty]
        public partial string? Unidad { get; set; }

        [ObservableProperty]
        public partial string? Observaciones { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorMedicamento))]
        public partial string? ErrorMedicamento { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorDosis))]
        public partial string? ErrorDosis { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HayErrorUnidad))]
        public partial string? ErrorUnidad { get; set; }

        public bool HayErrorMedicamento => !string.IsNullOrEmpty(ErrorMedicamento);
        public bool HayErrorDosis => !string.IsNullOrEmpty(ErrorDosis);
        public bool HayErrorUnidad => !string.IsNullOrEmpty(ErrorUnidad);

        public bool HayErrores => HayErrorMedicamento || HayErrorDosis || HayErrorUnidad;

        // Catálogo recargado: se conserva el medicamento elegido (por id) si sigue activo
        public void ActualizarMedicamentos(IReadOnlyList<Medicamento> medicamentos)
        {
            var idAnterior = MedicamentoSeleccionado?.Id;
            Medicamentos = medicamentos;
            MedicamentoSeleccionado = medicamentos.FirstOrDefault(m => m.Id == idAnterior);
        }

        // Marca cada campo con su error. Devuelve true si la fila está bien
        public bool Validar()
        {
            ErrorMedicamento = MedicamentoSeleccionado is null ? "Elegí un medicamento o quitá la fila." : null;

            var hayDosis = !string.IsNullOrWhiteSpace(Dosis);

            // R9: si hay dosis, es un número mayor a 0
            ErrorDosis = !hayDosis ? null
                : !TryParseDosis(out var dosis) ? "La dosis debe ser un número."
                : dosis <= 0 ? "La dosis debe ser mayor a 0."
                : null;

            // R10: si hay dosis, la unidad es obligatoria
            ErrorUnidad = hayDosis && string.IsNullOrEmpty(Unidad)
                ? "Elegí la unidad de la dosis."
                : null;

            return !HayErrores;
        }

        // Llamar después de Validar() con resultado true
        public DetalleMedicamentoRequest ArmarPedido()
        {
            var observaciones = Observaciones?.Trim();
            decimal? dosis = TryParseDosis(out var valor) ? valor : null;

            return new DetalleMedicamentoRequest
            {
                IdMedicamento = MedicamentoSeleccionado!.Id,
                // R10: sin dosis, dosis y unidad viajan en null
                Dosis = dosis,
                Unidad = dosis is null ? null : Unidad,
                Observaciones = string.IsNullOrEmpty(observaciones) ? null : observaciones
            };
        }

        // Mismo criterio que el peso: coma o punto como separador decimal, sin separador de miles
        private bool TryParseDosis(out decimal dosis)
        {
            var normalizado = Dosis?.Trim().Replace(',', '.');
            return decimal.TryParse(normalizado, NumberStyles.AllowDecimalPoint,
                CultureInfo.InvariantCulture, out dosis);
        }
    }
}
