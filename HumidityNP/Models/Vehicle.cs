using System;
using System.Text.Json.Serialization;

namespace HumidityNP.Models;

/// <summary>
/// Машина, въезжающая на площадку (данные из API).
/// </summary>
public class Vehicle
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Номер (Я-9310099848)</summary>
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    /// <summary>Дата создания записи</summary>
    [JsonPropertyName("date")]
    public DateTimeOffset Date { get; set; }

    /// <summary>Дата приезда</summary>
    [JsonPropertyName("arrivalDate")]
    public DateTimeOffset ArrivalDate { get; set; }

    /// <summary>Дата въезда</summary>
    [JsonPropertyName("entryDate")]
    public DateTimeOffset EntryDate { get; set; }

    /// <summary>Дата выезда</summary>
    [JsonPropertyName("exitDate")]
    public DateTimeOffset? ExitDate { get; set; }

    /// <summary>Контрагент</summary>
    [JsonPropertyName("counterparty")]
    public string Counterparty { get; set; } = string.Empty;

    /// <summary>Вид работ</summary>
    [JsonPropertyName("workType")]
    public string WorkType { get; set; } = string.Empty;

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

    /// <summary>Грузчик</summary>
    [JsonPropertyName("loader")]
    public string Loader { get; set; } = string.Empty;

    /// <summary>Экспедитор</summary>
    [JsonPropertyName("expeditor")]
    public string Expeditor { get; set; } = string.Empty;

    /// <summary>Подразделение</summary>
    [JsonPropertyName("department")]
    public string Department { get; set; } = string.Empty;

    /// <summary>Отображаемое имя</summary>
    [JsonIgnore]
    public string DisplayName => $"{VehicleBrand} {VehiclePlate}".Trim();

    /// <summary>Краткая информация</summary>
    [JsonIgnore]
    public string Summary => $"{Number} | {Counterparty} | {WorkType}";
}