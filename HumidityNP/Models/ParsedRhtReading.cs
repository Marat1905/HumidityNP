using HumidityNP.Enums;
using System;

namespace HumidityNP.Models;

/// <summary>
/// Результат разбора 12-байтного пакета RHT Reading.
/// Содержит данные измерения относительной влажности и температуры.
/// </summary>
/// <remarks>
/// Формат данных RHT Reading (12 байт):
/// - Byte 0: Reading Type (0x11)
/// - Byte 1: Location Setting (номер локации 1-99)
/// - Bytes 2-3: Relative Humidity Reading (0-1000 в единицах 0.1% RH)
/// - Bytes 4-5: Temperature Reading (-900 to 3000 в единицах 0.1°F, signed)
/// - Byte 6: Timestamp Second (0-59)
/// - Byte 7: Timestamp Minute (0-59)
/// - Byte 8: Timestamp Hour (1-12 + PM flag в 6-м бите)
/// - Byte 9: Timestamp Day (1-31)
/// - Byte 10: Timestamp Month (1-12)
/// - Byte 11: Timestamp Year (0-99)
/// </remarks>
public class ParsedRhtReading
{
    /// <summary>
    /// Тип измерения (всегда 0x11 для RHT).
    /// </summary>
    public ReadingType Type { get; set; } = ReadingType.RHT;

    /// <summary>
    /// Номер локации, где было выполнено измерение (1-99).
    /// </summary>
    public byte Location { get; set; }

    /// <summary>
    /// Относительная влажность в процентах (0.0-100.0% RH).
    /// Сырое значение 0-1000 с множителем 0.1.
    /// </summary>
    public double RelativeHumidity { get; set; }

    /// <summary>
    /// Температура в градусах Фаренгейта (-90.0 to 300.0°F).
    /// Сырое значение -900 to 3000 с множителем 0.1.
    /// </summary>
    public double TemperatureF { get; set; }

    /// <summary>
    /// Температура в градусах Цельсия.
    /// Вычисляется из TemperatureF.
    /// </summary>
    public double TemperatureC => (TemperatureF - 32) * 5.0 / 9.0;

    /// <summary>
    /// Секунды временной метки (0-59).
    /// </summary>
    public byte TimestampSecond { get; set; }

    /// <summary>
    /// Минуты временной метки (0-59).
    /// </summary>
    public byte TimestampMinute { get; set; }

    /// <summary>
    /// Часы временной метки (1-12 + PM flag в 6-м бите).
    /// </summary>
    public byte TimestampHour { get; set; }

    /// <summary>
    /// День временной метки (1-31).
    /// </summary>
    public byte TimestampDay { get; set; }

    /// <summary>
    /// Месяц временной метки (1-12).
    /// </summary>
    public byte TimestampMonth { get; set; }

    /// <summary>
    /// Год временной метки (0-99).
    /// </summary>
    public byte TimestampYear { get; set; }

    /// <summary>
    /// Возвращает true, если время указано в PM (после полудня).
    /// </summary>
    public bool IsPM => (TimestampHour & 0x20) != 0;

    /// <summary>
    /// Возвращает час в 24-часовом формате.
    /// </summary>
    public int Hour24 => IsPM ? (TimestampHour & 0x1F) + 12 : (TimestampHour & 0x1F);

    /// <summary>
    /// Временная метка в виде DateTimeOffset.
    /// Год интерпретируется как 2000+ для значений 0-49 и 1900+ для 50-99.
    /// </summary>
    public DateTimeOffset Timestamp
    {
        get
        {
            int year = TimestampYear < 50 ? 2000 + TimestampYear : 1900 + TimestampYear;
            return new DateTimeOffset(year, TimestampMonth, TimestampDay, Hour24, TimestampMinute, TimestampSecond, TimeSpan.Zero);
        }
    }

    /// <summary>
    /// Возвращает строковое представление данных RHT измерения.
    /// </summary>
    /// <returns>Форматированная строка с влажностью, температурой и временной меткой.</returns>
    public override string ToString()
    {
        return $"Влажность: {RelativeHumidity:F1}% RH; Температура: {TemperatureC:F1}°C ({TemperatureF:F1}°F); " +
               $"Локация: {Location}; Время: {TimestampDay:D2}.{TimestampMonth:D2}.{TimestampYear:D2} " +
               $"{Hour24:D2}:{TimestampMinute:D2}:{TimestampSecond:D2}";
    }
}