using System.Globalization;
using Microsoft.Maui.Controls;

namespace HumidityNP.Converters
{
    public class BoolToColorConverter : IValueConverter, IMarkupExtension
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isConnected = (bool)value;
            if (parameter is string param && param == "button")
            {
                return isConnected ? Color.FromArgb("#D32F2F") : Color.FromArgb("#1976D2");
            }
            return isConnected ? Color.FromArgb("#4CAF50") : Color.FromArgb("#F44336");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}