using System;
using System.Text.Json.Serialization;

namespace HumidityNP.Models;

/// <summary>
/// Машина, въезжающая на площадку (данные из API).
/// Соответствует VehicleDto на сервере.
/// </summary>
public class Vehicle
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Номер пропуска (Я-9310099848)</summary>
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    /// <summary>Дата создания пропуска</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    /// <summary>Дата въезда</summary>
    [JsonPropertyName("entryDate")]
    public DateTimeOffset EntryDate { get; set; }

    /// <summary>Дата выезда (может быть null)</summary>
    [JsonPropertyName("exitDate")]
    public DateTimeOffset? ExitDate { get; set; }

    /// <summary>Поставщик (контрагент)</summary>
    [JsonPropertyName("counterparty")]
    public string Counterparty { get; set; } = string.Empty;

    /// <summary>ИНН поставщика</summary>
    [JsonPropertyName("inn")]
    public string? Inn { get; set; }

    /// <summary>Марка авто</summary>
    [JsonPropertyName("vehicleBrand")]
    public string VehicleBrand { get; set; } = string.Empty;

    /// <summary>Гос. номер авто</summary>
    [JsonPropertyName("vehiclePlate")]
    public string VehiclePlate { get; set; } = string.Empty;

    /// <summary>Прицеп</summary>
    [JsonPropertyName("trailer")]
    public string Trailer { get; set; } = string.Empty;

    /// <summary>Водитель</summary>
    [JsonPropertyName("driver")]
    public string Driver { get; set; } = string.Empty;

    /// <summary>Количество замеров для этой машины</summary>
    [JsonPropertyName("measurementsCount")]
    public int MeasurementsCount { get; set; }

    /// <summary>Отображаемое имя (марка + госномер)</summary>
    [JsonIgnore]
    public string DisplayName => $"{VehicleBrand} {VehiclePlate}".Trim();

    /// <summary>Краткая информация (номер заявки | контрагент)</summary>
    [JsonIgnore]
    public string Summary => $"{Number} | {Counterparty}";
}