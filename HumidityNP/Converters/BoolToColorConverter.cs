using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace HumidityNP.Converters;

/// <summary>
/// Универсальный конвертер для преобразования различных значений в Color.
/// Поддерживает несколько режимов работы в зависимости от типа входного значения
/// и параметра ConverterParameter:
/// 
/// 1. bool → Color (индикатор подключения):
///    - true → зелёный (#4CAF50)
///    - false → красный (#F44336)
/// 
/// 2. bool + ConverterParameter="button" → Color (кнопка подключения):
///    - true → зелёный (#388E3C)
///    - false → синий (#1976D2)
/// 
/// 3. int/enum (Source) → Color (индикатор источника замера):
///    - 0 (Auto) → синий (#2196F3)
///    - 1 (Manual) → оранжевый (#FF9800)
/// 
/// 4. string → Color (по имени цвета):
///    - Возвращает Color по имени (например, "Red", "Blue")
/// </summary>
public class BoolToColorConverter : IValueConverter
{
    /// <summary>
    /// Цвет "подключено" (зелёный) для индикатора.
    /// </summary>
    private static readonly Color ConnectedColor = Color.FromArgb("#4CAF50");

    /// <summary>
    /// Цвет "отключено" (красный) для индикатора.
    /// </summary>
    private static readonly Color DisconnectedColor = Color.FromArgb("#F44336");

    /// <summary>
    /// Цвет кнопки "подключить" (синий).
    /// </summary>
    private static readonly Color ConnectButtonColor = Color.FromArgb("#1976D2");

    /// <summary>
    /// Цвет кнопки "отключить" (тёмно-зелёный).
    /// </summary>
    private static readonly Color DisconnectButtonColor = Color.FromArgb("#388E3C");

    /// <summary>
    /// Цвет для автоматического замера (синий).
    /// </summary>
    private static readonly Color AutoSourceColor = Color.FromArgb("#2196F3");

    /// <summary>
    /// Цвет для ручного замера (оранжевый).
    /// </summary>
    private static readonly Color ManualSourceColor = Color.FromArgb("#FF9800");

    /// <summary>
    /// Цвет по умолчанию (серый), если не удалось определить тип.
    /// </summary>
    private static readonly Color DefaultColor = Color.FromArgb("#9E9E9E");

    /// <summary>
    /// Преобразует входное значение в Color.
    /// </summary>
    /// <param name="value">Входное значение (bool, int, enum, string)</param>
    /// <param name="targetType">Целевой тип (должен быть Color)</param>
    /// <param name="parameter">Дополнительный параметр:
    /// - "button" → режим кнопки подключения
    /// - "indicator" → режим индикатора (по умолчанию для bool)
    /// - "source" → режим источника замера
    /// </param>
    /// <param name="culture">Культура (не используется)</param>
    /// <returns>Color соответствующий входному значению</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Если значение null — возвращаем цвет по умолчанию
        if (value == null)
        {
            return DefaultColor;
        }

        // Получаем параметр (приводим к строке, если возможно)
        string param = parameter?.ToString()?.ToLowerInvariant() ?? string.Empty;

        // ============================================
        // РЕЖИМ 1: Обработка bool (подключение)
        // ============================================
        if (value is bool boolValue)
        {
            // Если указан параметр "button" — возвращаем цвета для кнопок
            if (param == "button")
            {
                return boolValue ? DisconnectButtonColor : ConnectButtonColor;
            }

            // По умолчанию — цвета для индикатора
            return boolValue ? ConnectedColor : DisconnectedColor;
        }

        // ============================================
        // РЕЖИМ 2: Обработка int (Source = 0 или 1)
        // ============================================
        if (value is int intValue)
        {
            // 0 = Auto (синий), 1 = Manual (оранжевый)
            return intValue switch
            {
                0 => AutoSourceColor,
                1 => ManualSourceColor,
                _ => DefaultColor
            };
        }

        // ============================================
        // РЕЖИМ 3: Обработка enum (MeasurementSource)
        // ============================================
        if (value is Enum enumValue)
        {
            // Преобразуем enum в int и обрабатываем как int
            int underlyingValue = System.Convert.ToInt32(enumValue);
            return underlyingValue switch
            {
                0 => AutoSourceColor,
                1 => ManualSourceColor,
                _ => DefaultColor
            };
        }

        // ============================================
        // РЕЖИМ 4: Обработка string (имя цвета)
        // ============================================
        if (value is string stringValue)
        {
            // Пытаемся преобразовать строку в Color
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return DefaultColor;
            }

            // Пробуем распарсить как HEX цвет (#RRGGBB или #AARRGGBB)
            if (stringValue.StartsWith("#"))
            {
                try
                {
                    return Color.FromArgb(stringValue);
                }
                catch
                {
                    return DefaultColor;
                }
            }

            // Пробуем найти по имени (Red, Blue, Green и т.д.)
            try
            {
                // Используем reflection для поиска цвета по имени
                var colorProperty = typeof(Colors).GetProperty(stringValue,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.IgnoreCase);

                if (colorProperty != null)
                {
                    return (Color)colorProperty.GetValue(null);
                }
            }
            catch
            {
                // Игнорируем ошибки и возвращаем цвет по умолчанию
            }

            return DefaultColor;
        }

        // ============================================
        // РЕЖИМ 5: Обработка byte (для enum с underlying type byte)
        // ============================================
        if (value is byte byteValue)
        {
            return byteValue switch
            {
                0 => AutoSourceColor,
                1 => ManualSourceColor,
                _ => DefaultColor
            };
        }

        // Если тип не распознан — возвращаем цвет по умолчанию
        return DefaultColor;
    }

    /// <summary>
    /// Обратное преобразование (не реализовано, так как используется только в OneWay binding).
    /// </summary>
    /// <param name="value">Исходное значение</param>
    /// <param name="targetType">Целевой тип</param>
    /// <param name="parameter">Дополнительный параметр</param>
    /// <param name="culture">Культура</param>
    /// <returns>Не реализовано</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException("Обратное преобразование не требуется для BoolToColorConverter.");
    }
}