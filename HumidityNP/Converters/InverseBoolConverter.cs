using System.Globalization;
using Microsoft.Maui.Controls;

namespace HumidityNP.Converters;

public class InverseBoolConverter : IValueConverter, IMarkupExtension
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => !(bool)value;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public object ProvideValue(IServiceProvider serviceProvider) => this;
}