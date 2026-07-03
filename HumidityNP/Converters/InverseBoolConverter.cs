using System.Globalization;
using Microsoft.Maui.Controls;

namespace HumidityNP.Converters;

/// <summary>
/// Конвертер для инвертирования булевых значений.
/// Используется для отображения элементов, когда условие ложно.
/// Например, показ "Ожидание данных" когда IsDataAvailable == false.
/// </summary>
public class InverseBoolConverter : IValueConverter, IMarkupExtension
{
    /// <summary>
    /// Инвертирует булево значение.
    /// </summary>
    /// <param name="value">Исходное булево значение</param>
    /// <param name="targetType">Целевой тип (не используется)</param>
    /// <param name="parameter">Параметр (не используется)</param>
    /// <param name="culture">Культура (не используется)</param>
    /// <returns>Инвертированное булево значение</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        // Для числовых значений (например, Count == 0 → true)
        if (value is int intValue)
        {
            return intValue == 0;
        }

        return true;
    }

    /// <summary>
    /// Обратное преобразование не поддерживается.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не поддерживается");
    }

    /// <summary>
    /// Возвращает экземпляр конвертера для использования в XAML.
    /// </summary>
    public object ProvideValue(IServiceProvider serviceProvider) => this;
}