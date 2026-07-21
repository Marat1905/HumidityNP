using System;
using System.Text.Json.Serialization;
using HumidityNP.Enums;

namespace HumidityNP.Models;

/// <summary>
/// Запрос на создание замера.
/// </summary>
public class CreateMeasurementRequest
{
    /// <summary>
    /// Идентификатор машины.
    /// </summary>
    [JsonPropertyName("vehicleId")]
    public Guid VehicleId { get; set; }

    /// <summary>
    /// Значение влажности (%).
    /// </summary>
    [JsonPropertyName("humidityValue")]
    public double HumidityValue { get; set; }

    /// <summary>
    /// Температура (°C).
    /// </summary>
    [JsonPropertyName("temperatureC")]
    public double TemperatureC { get; set; }

    /// <summary>
    /// Тип измерения.
    /// </summary>
    [JsonPropertyName("measurementType")]
    public string MeasurementType { get; set; } = string.Empty;

    /// <summary>
    /// Материал.
    /// </summary>
    [JsonPropertyName("material")]
    public string Material { get; set; } = string.Empty;

    /// <summary>
    /// Источник данных (Auto/Manual).
    /// </summary>
    [JsonPropertyName("source")]
    public MeasurementSource Source { get; set; }

    /// <summary>
    /// Дата и время замера.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Знак (Less/Greater/None).
    /// </summary>
    [JsonPropertyName("sign")]
    public SignType Sign { get; set; }
}