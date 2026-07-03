using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace HumidityNP.Converters;

/// <summary>
/// Конвертер для преобразования источника замера (Source) в иконку.
/// - 0 (Auto) → 📡 (автоматический замер с датчика)
/// - 1 (Manual) → ✋ (ручной ввод)
/// </summary>
public class SourceToIconConverter : IValueConverter
{
    /// <summary>
    /// Преобразует значение Source в строку с иконкой.
    /// </summary>
    /// <param name="value">Значение Source (int, enum или bool)</param>
    /// <param name="targetType">Целевой тип (string)</param>
    /// <param name="parameter">Дополнительный параметр (не используется)</param>
    /// <param name="culture">Культура (не используется)</param>
    /// <returns>Строка с иконкой: "📡" для Auto, "✋" для Manual</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return "📡";
        }

        // Обработка int
        if (value is int intValue)
        {
            return intValue switch
            {
                0 => "📡",  // Auto
                1 => "✋",  // Manual
                _ => "📡"
            };
        }

        // Обработка enum
        if (value is Enum enumValue)
        {
            int underlyingValue = System.Convert.ToInt32(enumValue);
            return underlyingValue switch
            {
                0 => "📡",
                1 => "✋",
                _ => "📡"
            };
        }

        // Обработка byte
        if (value is byte byteValue)
        {
            return byteValue switch
            {
                0 => "📡",
                1 => "✋",
                _ => "📡"
            };
        }

        return "📡";
    }

    /// <summary>
    /// Обратное преобразование (не реализовано).
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не требуется для SourceToIconConverter.");
    }
}