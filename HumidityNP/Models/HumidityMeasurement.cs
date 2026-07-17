using HumidityNP.Enums;
using SQLite;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumidityNP.Models;

/// <summary>
/// Замер влажности, привязанный к машине.
/// Хранится локально в SQLite до выгрузки на сервер.
/// </summary>
[SQLite.Table("humidity_measurements")]
public class HumidityMeasurement
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }

    /// <summary>ID машины (связь с Vehicle.Id)</summary>
    [Indexed]
    public string VehicleId { get; set; } = string.Empty;

    /// <summary>Числовое значение влажности (%)</summary>
    public double HumidityValue { get; set; }

    /// <summary>Температура в °C</summary>
    public double TemperatureC { get; set; }

    /// <summary>Тип измерения (из BLE-протокола)</summary>
    public string MeasurementType { get; set; } = string.Empty;

    /// <summary>Материал</summary>
    public string Material { get; set; } = string.Empty;

    /// <summary>Источник данных: Auto (датчик) или Manual (вручную)</summary>
    public MeasurementSource Source { get; set; }

    /// <summary>Дата и время замера</summary>
    public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>Знак (Less/Greater/None) для BLE-данных</summary>
    public string Sign { get; set; } = string.Empty;

    /// <summary>
    /// Локальное время замера для отображения в UI.
    /// Автоматически конвертируется в часовой пояс устройства.
    /// </summary>
    [NotMapped]
    [Ignore]
    public DateTimeOffset LocalTimestamp => Timestamp.ToLocalTime();


    /// <summary>Отображаемое значение влажности</summary>
    public string DisplayValue
    {
        get
        {
            string sign = Sign == "Less" ? "<" : Sign == "Greater" ? ">" : "";
            return $"{sign}{HumidityValue:F1}%";
        }
    }

    /// <summary>Отображаемый источник</summary>
    public string SourceDisplay => Source == MeasurementSource.Auto ? "📡 Датчик" : "✋ Вручную";
}