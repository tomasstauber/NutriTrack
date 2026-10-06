using System.Globalization;

namespace NutriTrack.MAUI.Helpers
{
    // Muestra "—" cuando el valor es null o un texto vacío (columnas opcionales de las tablas)
    public class NuloAGuionConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var texto = value?.ToString();
            return string.IsNullOrWhiteSpace(texto) ? "—" : texto;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotSupportedException();
    }
}
