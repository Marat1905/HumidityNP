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
    public string DisplayName => $"{VehicleBrand?.ToUpperInvariant() ?? string.Empty} {VehiclePlate?.ToUpperInvariant().Replace(" ", string.Empty) ?? string.Empty}".Trim();

    /// <summary>
    /// Строка с номером заявки и номером прицепа.
    /// Используется вместо Summary на странице списка машин.
    /// </summary>
    [JsonIgnore]
    public string TicketAndTrailer => $"{Number} | Прицеп:{Trailer?.ToUpperInvariant().Replace(" ", string.Empty) ?? string.Empty}";

    /// <summary>
    /// ФИО водителя с нормализованным регистром (каждое слово с заглавной буквы).
    /// </summary>
    [JsonIgnore]
    public string DriverDisplay
    {
        get
        {
            if (string.IsNullOrWhiteSpace(Driver))
                return Driver;

            // Разбиваем строку на слова по пробелам
            var words = Driver.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < words.Length; i++)
            {
                // Если слово не пустое, делаем первую букву заглавной, остальные строчными
                if (words[i].Length > 0)
                {
                    words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
                }
            }
            return string.Join(" ", words);
        }
    }
}